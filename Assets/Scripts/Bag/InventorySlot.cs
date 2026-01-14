using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [Header("UI组件")]
    public Image iconImage;
    public TextMeshProUGUI countText;

    [Header("物品图标映射")]
    public ItemIconMapping[] iconMappings;

    [System.Serializable]
    public class ItemIconMapping
    {
        public string itemID;
        public Sprite icon;
        public string displayName;
    }

    public void SetItem(string itemID, int count)
    {
        // 查找图标
        Sprite icon = null;

        if (iconMappings != null)
        {
            foreach (ItemIconMapping mapping in iconMappings)
            {
                if (mapping.itemID == itemID)
                {
                    icon = mapping.icon;
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
                // 没有图标时显示默认颜色
                iconImage.sprite = null;
                iconImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);
                iconImage.enabled = true;
            }
        }

        // 设置数量
        if (countText != null)
        {
            if (count > 1)
            {
                countText.text = count.ToString();
                countText.enabled = true;
            }
            else
            {
                countText.enabled = false;
            }
        }
    }

    public void ClearSlot()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        }

        if (countText != null)
        {
            countText.enabled = false;
        }
    }
}