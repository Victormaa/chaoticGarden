using UnityEngine;

[CreateAssetMenu(fileName = "NewCrop", menuName = "Farm/Crop Data")]
public class CropData : ScriptableObject
{
    [Header("基本信息")]
    public string cropID; // 唯一ID，既是作物ID也是种子ID
    public string cropName;
    public Sprite icon;
    public GameObject cropPrefab;

    [Header("生长配置")]
    public float totalGrowthTime = 20f;  // 总生长时间
    public CropGrowthStage[] growthStages;

    [Header("收获配置")]
    public int harvestAmount = 1; // 直接收获作物本身，无需单独收获ID

    // 验证方法
    void OnValidate()
    {
        if (string.IsNullOrEmpty(cropID))
            Debug.LogWarning($"CropData '{name}' 缺少 cropID");

        if (cropPrefab == null)
            Debug.LogWarning($"CropData '{name}' 缺少 cropPrefab");

        if (growthStages == null || growthStages.Length == 0)
            Debug.LogWarning($"CropData '{name}' 缺少生长阶段");

        if (totalGrowthTime <= 0)
            Debug.LogWarning($"CropData '{name}' 的生长时间必须大于0");
    }
}

[System.Serializable]
public class CropGrowthStage
{
    public string stageName;
    public GameObject model;
    public Vector3 scale = Vector3.one;
}