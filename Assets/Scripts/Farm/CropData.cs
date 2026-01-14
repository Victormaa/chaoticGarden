using UnityEngine;

[CreateAssetMenu(fileName = "NewCrop", menuName = "Farming/Crop Data")]
public class CropData : ScriptableObject
{
    [Header("Basic Information")]//基本信息
    public string cropName = "Crop";
    public string itemID = "crop_item";
    public Sprite icon;

    [Header("Growth Models")]
    public GameObject seedModel;
    public GameObject growingModel;
    public GameObject matureModel;

    [Header("Model Position Offset")]
    [Tooltip("Position offset for seed stage")]
    public Vector3 seedPositionOffset = new Vector3(0, 0.1f, 0);

    [Tooltip("Position offset for growing stage")]
    public Vector3 growingPositionOffset = new Vector3(0, 0.2f, 0);

    [Tooltip("Position offset for mature stage")]
    public Vector3 maturePositionOffset = new Vector3(0, 0.3f, 0);

    [Header("Model Rotation")]
    public Vector3 seedRotation = Vector3.zero;
    public Vector3 growingRotation = Vector3.zero;
    public Vector3 matureRotation = Vector3.zero;

    [Header("Model Scale")]
    public Vector3 seedScale = Vector3.one;
    public Vector3 growingScale = Vector3.one;
    public Vector3 matureScale = Vector3.one;

    [Header("Growth Time")]
    public float seedGrowthTime = 3f;
    public float seedlingGrowthTime = 10f;

    [Header("Harvest Settings")]//收获设置
    public string harvestItemID = "crop_harvest";
    public int harvestAmount = 3;
}