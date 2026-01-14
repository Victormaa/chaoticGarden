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
}