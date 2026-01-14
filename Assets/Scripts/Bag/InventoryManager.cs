using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class InventoryItem
{
    public string itemID;
    public int quantity;
}

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<InventoryItem> items = new List<InventoryItem>();

    // 背包变化事件，UI会监听这个事件
    public event Action OnInventoryChanged;

    void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool HasItem(string itemID, int amount = 1)
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemID == itemID && item.quantity >= amount)
            {
                return true;
            }
        }
        return false;
    }

    public int GetItemCount(string itemID)
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemID == itemID)
            {
                return item.quantity;
            }
        }
        return 0;
    }

    public void AddItem(string itemID, int amount = 1)
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemID == itemID)
            {
                item.quantity += amount;
                Debug.Log("添加物品：" + itemID + " x" + amount + "，当前数量：" + item.quantity);

                // 通知UI更新
                OnInventoryChanged?.Invoke();
                return;
            }
        }

        items.Add(new InventoryItem { itemID = itemID, quantity = amount });
        Debug.Log("添加新物品：" + itemID + " x" + amount);

        // 通知UI更新
        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(string itemID, int amount = 1)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].itemID == itemID)
            {
                items[i].quantity -= amount;

                if (items[i].quantity <= 0)
                {
                    items.RemoveAt(i);
                    Debug.Log("物品已用完：" + itemID);
                }
                else
                {
                    Debug.Log("移除物品：" + itemID + " x" + amount + "，剩余数量：" + items[i].quantity);
                }

                // 通知UI更新
                OnInventoryChanged?.Invoke();
                return;
            }
        }
    }

    public List<InventoryItem> GetAllItems()
    {
        return items;
    }

    public void ClearInventory()
    {
        items.Clear();
        OnInventoryChanged?.Invoke();
    }
}