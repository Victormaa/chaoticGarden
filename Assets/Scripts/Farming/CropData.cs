using UnityEngine;

/// <summary>
/// Crop data
/// </summary>
[CreateAssetMenu(fileName = "NewCrop", menuName = "Farming/Crop Data")]
public class CropData : ItemData
{
    [Header("CropMessage")]
    [Tooltip("CostMessage")]
    public int cropValue = 10;

    [Tooltip("CropLevel")]
    [Range(1, 5)]
    public int quality = 1;

    [Header("3D模型阶段")]
    [Tooltip("生长阶段的预制体")]
    public GameObject[] growthStagePrefabs;

    [Header("Color")]
    public Color growingColor = Color.gray;
    public Color matureColor = Color.green;

    void OnValidate()
    {
        itemType = ItemType.Crop;
    }
}
