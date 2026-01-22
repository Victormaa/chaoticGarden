using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("基本信息")]
    public string itemID;
    public string itemName;
    public Sprite icon;

    [Header("描述")]
    [TextArea(2, 4)]
    public string description;

    [Header("堆叠")]
    public int maxStackSize = 99;
    public List<ItemData> items = new List<ItemData>();
    // 示例：胡萝卜物品数据
    [CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item Data")]
    public class ItemID : ScriptableObject
    {
        public string itemID = "Beet_1";  // 与 CropData 的 cropID 一致
        public string itemName = "Beet";
        public Sprite icon;
        public int maxStack = 99;

    }
}