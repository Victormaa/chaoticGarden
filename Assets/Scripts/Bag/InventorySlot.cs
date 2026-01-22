using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [Header("UI组件")]
    public Image iconImage;              // 物品图标
    public TextMeshProUGUI countText;    // 数量文字

    [Header("物品图标映射")]
    public ItemIconMapping[] iconMappings;

    private string currentItemID;
    private int currentCount;

    [System.Serializable]
    public class ItemIconMapping
    {
        public string itemID;           // 物品ID（如 "Beet_1"）
        public Sprite icon;             // 物品图标
        public string displayName;      // 显示名称（可选）
    }

    public void SetItem(string itemID, int count)
    {
        currentItemID = itemID;
        currentCount = count;

        // 查找对应的图标
        Sprite icon = null;
        string displayName = itemID;

        if (iconMappings != null)
        {
            foreach (ItemIconMapping mapping in iconMappings)
            {
                if (mapping.itemID == itemID)
                {
                    icon = mapping.icon;
                    if (!string.IsNullOrEmpty(mapping.displayName))
                    {
                        displayName = mapping.displayName;
                    }
                    break;
                }
            }
        }

        // 设置图标
        if (iconImage != null)
        {
            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.color = Color.white;
                iconImage.enabled = true;
            }
            else
            {
                // 没有找到图标，显示默认颜色
                iconImage.sprite = null;
                iconImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                iconImage.enabled = true;
                Debug.LogWarning("No icon found for item: " + itemID);
            }
        }

        // 设置数量文字
        if (countText != null)
        {
            if (count > 1)
            {
                countText.text = count.ToString();
                countText.enabled = true;
            }
            else if (count == 1)
            {
                // 数量为1时可以选择不显示或显示
                countText.enabled = false; // 不显示
                // countText.text = "1";
                // countText.enabled = true; // 如果想显示1
            }
            else
            {
                countText.enabled = false;
            }
        }
    }

    public void ClearSlot()
    {
        currentItemID = "";
        currentCount = 0;

        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.color = new Color(0.3f, 0.3f, 0.3f, 0.5f); // 空格子的颜色
            iconImage.enabled = true;
        }

        if (countText != null)
        {
            countText.text = "";
            countText.enabled = false;
        }
    }
}