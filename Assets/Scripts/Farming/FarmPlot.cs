using UnityEngine;
using System.Collections;

/// <summary>
/// Field condition
/// </summary>
public enum PlotState
{
    Empty,      // 空地
    Planted,    // 已种植
    Growing,    // 生长中
    Mature,     // 成熟
    Withered    // 枯萎
}

/// <summary>
/// Farmland (the area suitable for cultivation)
/// </summary>
public class FarmPlot : MonoBehaviour
{
    [Header("田地状态")]
    public PlotState currentState = PlotState.Empty;

    [Header("当前作物")]
    public SeedData plantedSeed;
    public CropData currentCrop;

    [Header("生长进度")]
    [Range(0, 1)]
    public float growthProgress = 0f;
    public float growthTimer = 0f;

    [Header("3D Model")]
    public Transform cropModelParent; 
    private GameObject currentCropModel;

    [Header("视觉反馈")]
    public Renderer plotRenderer;
    public Color emptyColor = new Color(0.4f, 0.3f, 0.2f); 
    public Color plantedColor = new Color(0.3f, 0.25f, 0.15f);
    public Color matureColor = new Color(0.2f, 0.4f, 0.2f);

    [Header("交互")]
    public bool isPlayerNearby = false;
    public float interactionRange = 2f;

    void Start()
    {
        
        if (plotRenderer == null)
        {
            plotRenderer = GetComponent<Renderer>();
        }

        
        if (cropModelParent == null)
        {
            GameObject parent = new GameObject("CropModel");
            parent.transform.SetParent(transform);
            parent.transform.localPosition = Vector3.zero;
            cropModelParent = parent.transform;
        }

        UpdateVisuals();
    }

    void Update()
    {
        if (currentState == PlotState.Growing)
        {
            GrowCrop();
        }

        CheckPlayerNearby();
    }

    /// <summary>
    /// Detect whether the player is approaching.
    /// </summary>
    void CheckPlayerNearby()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            isPlayerNearby = distance <= interactionRange;
        }
        else
        {
            isPlayerNearby = false;
        }
    }

    /// <summary>
    /// Plant seeds
    /// </summary>
    public bool PlantSeed(SeedData seed)
    {
        if (currentState != PlotState.Empty)
        {
            Debug.LogWarning("This piece of land has already been planted with crops!");
            return false;
        }

        if (seed == null || seed.cropToGrow == null)
        {
            Debug.LogError("Invalid seed data!");
            return false;
        }

        // Set data
        plantedSeed = seed;
        currentCrop = seed.cropToGrow;
        currentState = PlotState.Growing;
        growthTimer = 0f;
        growthProgress = 0f;

        // Generate crop model
        SpawnCropModel();

        // Update the visuals
        UpdateVisuals();

        Debug.Log($"Planted in the field {seed.itemName}");
        return true;
    }

    /// <summary>
    /// Grow crops
    /// </summary>
    void GrowCrop()
    {
        if (plantedSeed == null) return;

        growthTimer += Time.deltaTime;
        growthProgress = Mathf.Clamp01(growthTimer / plantedSeed.growthTime);

        // Update the size of the crop model (growth animation)
        if (currentCropModel != null)
        {
            float scale = Mathf.Lerp(0.2f, 1f, growthProgress);
            currentCropModel.transform.localScale = Vector3.one * scale;
        }

        // Check if it is mature
        if (growthProgress >= 1f)
        {
            currentState = PlotState.Mature;
            UpdateVisuals();
            Debug.Log($"{currentCrop.itemName} It's mature!");
        }
    }

    /// <summary>
    /// Generate crop model
    /// </summary>
    void SpawnCropModel()
    {
        // 清除旧模型
        if (currentCropModel != null)
        {
            Destroy(currentCropModel);
        }

        if (currentCrop == null) return;

        // Use the first growth stage model (if available)
        GameObject prefab = null;
        if (currentCrop.growthStagePrefabs != null && currentCrop.growthStagePrefabs.Length > 0)
        {
            prefab = currentCrop.growthStagePrefabs[0];
        }
        else if (currentCrop.worldPrefab != null)
        {
            prefab = currentCrop.worldPrefab;
        }

        if (prefab != null)
        {
            currentCropModel = Instantiate(prefab, cropModelParent);
            currentCropModel.transform.localPosition = Vector3.up * 0.5f;
            currentCropModel.transform.localScale = Vector3.one * 0.2f; // 从小开始
        }
        else
        {
            // Create default model
            CreateDefaultCropModel();
        }
    }

    /// <summary>
    /// Create default crop model
    /// </summary>
    void CreateDefaultCropModel()
    {
        currentCropModel = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        currentCropModel.transform.SetParent(cropModelParent);
        currentCropModel.transform.localPosition = Vector3.up * 0.5f;
        currentCropModel.transform.localScale = Vector3.one * 0.2f;

        // Remove the collider
        Collider col = currentCropModel.GetComponent<Collider>();
        if (col != null) Destroy(col);

        // Set color
        Renderer rend = currentCropModel.GetComponent<Renderer>();
        if (rend != null && currentCrop != null)
        {
            rend.material.color = currentCrop.growingColor;
        }
    }

    /// <summary>
    /// Harvesting crops
    /// </summary>
    public bool HarvestCrop()
    {
        if (currentState != PlotState.Mature)
        {
            Debug.LogWarning("作物尚未成熟！");
            return false;
        }

        if (currentCrop == null || plantedSeed == null)
        {
            Debug.LogError("作物数据丢失！");
            return false;
        }

        // Add crops to the back
        bool success = InventorySystem.Instance.AddItem(currentCrop, plantedSeed.harvestYield);

        if (success)
        {
            // Optional: Return the seeds
            if (plantedSeed.seedsOnHarvest > 0)
            {
                InventorySystem.Instance.AddItem(plantedSeed, plantedSeed.seedsOnHarvest);
            }

            Debug.Log($"Reap {currentCrop.itemName} x{plantedSeed.harvestYield}");

            // Reset the fields
            ResetPlot();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Reset the fields
    /// </summary>
    void ResetPlot()
    {
        currentState = PlotState.Empty;
        plantedSeed = null;
        currentCrop = null;
        growthTimer = 0f;
        growthProgress = 0f;

        // Destroying crop model
        if (currentCropModel != null)
        {
            Destroy(currentCropModel);
            currentCropModel = null;
        }

        UpdateVisuals();
    }

    /// <summary>
    /// Update the visual effects
    /// </summary>
    void UpdateVisuals()
    {
        if (plotRenderer == null) return;

        Color targetColor = currentState switch
        {
            PlotState.Empty => emptyColor,
            PlotState.Planted => plantedColor,
            PlotState.Growing => Color.Lerp(plantedColor, matureColor, growthProgress),
            PlotState.Mature => matureColor,
            _ => emptyColor
        };

        plotRenderer.material.color = targetColor;

        // Update the color of the crop model
        if (currentCropModel != null && currentCrop != null)
        {
            Renderer cropRenderer = currentCropModel.GetComponent<Renderer>();
            if (cropRenderer != null)
            {
                if (currentState == PlotState.Mature)
                {
                    cropRenderer.material.color = currentCrop.matureColor;
                }
                else
                {
                    cropRenderer.material.color = currentCrop.growingColor;
                }
            }
        }
    }

    /// <summary>
    /// Gizmos Show
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isPlayerNearby ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);

        if (currentState == PlotState.Mature)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position + Vector3.up, Vector3.one * 0.5f);
        }
    }
}
