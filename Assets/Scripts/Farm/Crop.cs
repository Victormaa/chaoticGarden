using UnityEngine;

public class Crop : MonoBehaviour
{
    private CropData cropData;
    private GameObject commonSeedPrefab;
    private FarmLand farmLand;
    private GameObject currentModel;
    private CropStage currentStage = CropStage.Seed;
    private float stageTimer = 0f;

    public void Initialize(CropData data, GameObject seedPrefab, FarmLand land)
    {
        cropData = data;
        commonSeedPrefab = seedPrefab;
        farmLand = land;

        currentStage = CropStage.Seed;
        stageTimer = 0f;

        SpawnModel(CropStage.Seed);

        Debug.Log("Crop initialized: " + cropData.cropName);
    }

    void Update()
    {
        if (cropData == null) return;

        stageTimer += Time.deltaTime;

        switch (currentStage)
        {
            case CropStage.Seed:
                if (stageTimer >= cropData.seedGrowthTime)
                {
                    GrowToNextStage(CropStage.Seedling);
                }
                break;

            case CropStage.Seedling:
                if (stageTimer >= cropData.seedlingGrowthTime)
                {
                    GrowToNextStage(CropStage.Mature);
                }
                break;
        }
    }

    void GrowToNextStage(CropStage nextStage)
    {
        currentStage = nextStage;
        stageTimer = 0f;

        if (currentModel != null)
        {
            Destroy(currentModel);
        }

        SpawnModel(nextStage);

        Debug.Log("Crop entered " + nextStage + " stage");
    }

    void SpawnModel(CropStage stage)
    {
        GameObject modelPrefab = null;
        Vector3 positionOffset = Vector3.zero;
        Vector3 rotation = Vector3.zero;
        Vector3 scale = Vector3.one;

        switch (stage)
        {
            case CropStage.Seed:
                modelPrefab = cropData.seedModel != null ? cropData.seedModel : commonSeedPrefab;
                positionOffset = cropData.seedPositionOffset;
                rotation = cropData.seedRotation;
                scale = cropData.seedScale;
                break;

            case CropStage.Seedling:
                modelPrefab = cropData.growingModel;
                positionOffset = cropData.growingPositionOffset;
                rotation = cropData.growingRotation;
                scale = cropData.growingScale;
                break;

            case CropStage.Mature:
                modelPrefab = cropData.matureModel;
                positionOffset = cropData.maturePositionOffset;
                rotation = cropData.matureRotation;
                scale = cropData.matureScale;
                break;
        }

        if (modelPrefab == null)
        {
            CreateDefaultModel(stage, positionOffset, rotation, scale);
            return;
        }

        currentModel = Instantiate(modelPrefab, transform);

        currentModel.transform.localPosition = positionOffset;
        currentModel.transform.localRotation = Quaternion.Euler(rotation);
        currentModel.transform.localScale = scale;

        Debug.Log("Spawned " + stage + " model - Position: " + positionOffset + ", Rotation: " + rotation + ", Scale: " + scale);
    }

    void CreateDefaultModel(CropStage stage, Vector3 position, Vector3 rotation, Vector3 scale)
    {
        PrimitiveType primitiveType = PrimitiveType.Cube;
        Color color = Color.white;
        Vector3 defaultScale = Vector3.one * 0.1f;

        switch (stage)
        {
            case CropStage.Seed:
                primitiveType = PrimitiveType.Sphere;
                color = new Color(0.55f, 0.35f, 0.17f);
                defaultScale = new Vector3(0.1f, 0.1f, 0.1f);
                break;

            case CropStage.Seedling:
                primitiveType = PrimitiveType.Cube;
                color = new Color(0.2f, 0.8f, 0.2f);
                defaultScale = new Vector3(0.15f, 0.25f, 0.15f);
                break;

            case CropStage.Mature:
                primitiveType = PrimitiveType.Sphere;
                color = new Color(0.8f, 0.2f, 0.2f);
                defaultScale = new Vector3(0.25f, 0.25f, 0.25f);
                break;
        }

        currentModel = GameObject.CreatePrimitive(primitiveType);
        currentModel.transform.SetParent(transform);
        currentModel.transform.localPosition = position;
        currentModel.transform.localRotation = Quaternion.Euler(rotation);
        currentModel.transform.localScale = Vector3.Scale(defaultScale, scale);

        Renderer renderer = currentModel.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }

        Debug.LogWarning("Using default " + stage + " model");
    }

    public bool CanHarvest()
    {
        return currentStage == CropStage.Mature;
    }

    public void Harvest()
    {
        if (!CanHarvest())
        {
            Debug.LogWarning("Crop is not yet mature");
            return;
        }

        InventoryManager inventory = FindObjectOfType<InventoryManager>();
        if (inventory != null)
        {
            inventory.AddItem(cropData.harvestItemID, cropData.harvestAmount);
            Debug.Log("Harvested " + cropData.harvestAmount + " " + cropData.cropName);
        }

        Destroy(gameObject);
    }
}

public enum CropStage
{
    Seed,      // Seed
    Seedling,  // Seedling
    Mature     // Mature
}