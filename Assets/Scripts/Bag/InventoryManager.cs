using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("背包设置")]
    public int maxSlots = 20;
    public int maxStackSize = 99;

    [Header("UI引用")]
    public InventoryUI inventoryUI;

    // 事件
    public System.Action OnInventoryChanged;

    private List<InventoryItem> items = new List<InventoryItem>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // 添加物品
    public bool AddItem(string itemID, int amount)
    {
        if (string.IsNullOrEmpty(itemID) || amount <= 0) return false;

        int remaining = amount;

        // 优先堆叠到已有物品
        foreach (var item in items)
        {
            if (item.itemID == itemID && item.quantity < maxStackSize)
            {
                int canAdd = Mathf.Min(remaining, maxStackSize - item.quantity);
                item.quantity += canAdd;
                remaining -= canAdd;

                if (remaining <= 0) break;
            }
        }

        // 剩余的放到新槽位
        while (remaining > 0)
        {
            if (items.Count >= maxSlots)
            {
                Debug.LogWarning("背包已满！");
                OnInventoryChanged?.Invoke();
                return false;
            }

            int canAdd = Mathf.Min(remaining, maxStackSize);
            items.Add(new InventoryItem { itemID = itemID, quantity = canAdd });
            remaining -= canAdd;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    // 移除物品
    public bool RemoveItem(string itemID, int amount)
    {
        if (string.IsNullOrEmpty(itemID) || amount <= 0) return false;

        int remaining = amount;

        for (int i = items.Count - 1; i >= 0 && remaining > 0; i--)
        {
            if (items[i].itemID == itemID)
            {
                if (items[i].quantity >= remaining)
                {
                    items[i].quantity -= remaining;
                    remaining = 0;

                    if (items[i].quantity <= 0)
                    {
                        items.RemoveAt(i);
                    }
                }
                else
                {
                    remaining -= items[i].quantity;
                    items.RemoveAt(i);
                }
            }
        }

        OnInventoryChanged?.Invoke();
        return remaining <= 0;
    }

    // 获取物品数量
    public int GetItemCount(string itemID)
    {
        if (string.IsNullOrEmpty(itemID)) return 0;

        int total = 0;
        foreach (var item in items)
        {
            if (item.itemID == itemID)
            {
                total += item.quantity;
            }
        }
        return total;
    }

    // 获取所有物品
    public List<InventoryItem> GetAllItems()
    {
        return items;
    }
}

// 物品数据类
[System.Serializable]
public class InventoryItem
{
    public string itemID;
    public int quantity;
}