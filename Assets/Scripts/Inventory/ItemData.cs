using UnityEngine;

/// <summary>
/// Itemtype
/// </summary>
public enum ItemType
{
    Seed,      // 种子
    Crop,      // 作物
    Tool,      // 工具
    Other      // 其他
}

/// <summary>
/// Item Data Base Class (ScriptableObject)
/// </summary>
[CreateAssetMenu(fileName = "NewItem", menuName = "Farming/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Information")]
    public string itemName = "NewPlants";
    public string itemID = "item_000";
    public ItemType itemType = ItemType.Other;

    [TextArea(2, 4)]
    public string description = "Article description";

    [Header("视觉")]
    public Sprite icon;
    public GameObject worldPrefab; // 3D模型

    [Header("属性")]
    public int maxStackSize = 99;
    public int sellPrice = 10;
    public bool canBeDropped = true;
}
