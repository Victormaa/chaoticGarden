using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Player Crop Harvester (3D)
/// </summary>
public class PlayerHarvester : MonoBehaviour
{
    [Header("Harvest Settings")]
    public float harvestRange = 3f;
    public KeyCode harvestKey = KeyCode.Space;
    public LayerMask cropLayer;

    [Header("Visual Feedback")]
    public GameObject harvestIndicatorPrefab;
    public Color canHarvestColor = Color.green;
    public Color cannotHarvestColor = Color.red;

    private GameObject currentIndicator;
    private CropBase nearestCrop;
    private List<CropBase> cropsInRange = new List<CropBase>();

    void Update()
    {
        ScanForCrops();
        UpdateIndicator();
        HandleInput();
    }

    /// <summary>
    /// Scan for crops within range
    /// </summary>
    void ScanForCrops()
    {
        cropsInRange.Clear();
        nearestCrop = null;

        Collider[] colliders =
            Physics.OverlapSphere(transform.position, harvestRange);

        float nearestDistance = float.MaxValue;

        foreach (var col in colliders)
        {
            CropBase crop = col.GetComponent<CropBase>();
            if (crop != null)
            {
                cropsInRange.Add(crop);

                float distance = Vector3.Distance(
                    transform.position,
                    crop.transform.position
                );

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestCrop = crop;
                }
            }
        }
    }

    /// <summary>
    /// Update harvest indicator
    /// </summary>
    void UpdateIndicator()
    {
        if (nearestCrop != null &&
            nearestCrop.currentState == CropState.Mature)
        {
            ShowIndicator(
                nearestCrop.transform.position,
                canHarvestColor
            );
        }
        else if (nearestCrop != null)
        {
            ShowIndicator(
                nearestCrop.transform.position,
                cannotHarvestColor
            );
        }
        else
        {
            HideIndicator();
        }
    }

    /// <summary>
    /// Show indicator
    /// </summary>
    void ShowIndicator(Vector3 position, Color color)
    {
        if (currentIndicator == null)
        {
            CreateIndicator();
        }

        currentIndicator.SetActive(true);
        currentIndicator.transform.position =
            position + Vector3.up * 0.1f;

        Renderer renderer =
            currentIndicator.GetComponent<Renderer>();

        if (renderer != null)
        {
            renderer.material.color = color;
        }

        // Rotation animation
        currentIndicator.transform.Rotate(
            Vector3.up,
            90f * Time.deltaTime
        );
    }

    void HideIndicator()
    {
        if (currentIndicator != null)
        {
            currentIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Create indicator (ring)
    /// </summary>
    void CreateIndicator()
    {
        if (harvestIndicatorPrefab != null)
        {
            currentIndicator =
                Instantiate(harvestIndicatorPrefab);
        }
        else
        {
            // Create temporary indicator (ring)
            currentIndicator =
                GameObject.CreatePrimitive(PrimitiveType.Cylinder);

            currentIndicator.name = "HarvestIndicator";

            // Adjust shape
            currentIndicator.transform.localScale =
                new Vector3(1.5f, 0.05f, 1.5f);

            // Remove collider
            Destroy(
                currentIndicator.GetComponent<Collider>()
            );

            // Semi-transparent material
            Material mat =
                new Material(Shader.Find("Standard"));

            mat.color = new Color(0f, 1f, 0f, 0.5f);
            mat.SetFloat("_Mode", 3); // Transparent
            mat.SetInt(
                "_SrcBlend",
                (int)UnityEngine.Rendering.BlendMode.SrcAlpha
            );
            mat.SetInt(
                "_DstBlend",
                (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha
            );
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = 3000;

            currentIndicator.GetComponent<Renderer>().material =
                mat;
        }
    }

    /// <summary>
    /// Handle input
    /// </summary>
    void HandleInput()
    {
        if (Input.GetKeyDown(harvestKey))
        {
            if (nearestCrop != null &&
                nearestCrop.currentState == CropState.Mature)
            {
                nearestCrop.Harvest();
                HideIndicator();
            }
            else if (nearestCrop != null)
            {
                Debug.Log(
                    string.Format(
                        "{0} is not mature yet!",
                        nearestCrop.cropName
                    )
                );
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            transform.position,
            harvestRange
        );

        if (nearestCrop != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(
                transform.position,
                nearestCrop.transform.position
            );
        }
    }
}
