using UnityEngine;

[CreateAssetMenu(fileName = "New Collectible", menuName = "Farming/Collectible Data")]
public class CollectibleData : ScriptableObject
{
    public string itemID;              // 物品ID（与背包系统对应）
    public string itemName;            // 物品名称
    public Sprite icon;                // 物品图标
    public GameObject collectiblePrefab;  // 掉落物预制体
}