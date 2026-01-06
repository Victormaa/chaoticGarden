using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Backpack Slots
/// </summary>
[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int count;

    public InventorySlot(ItemData item, int count)
    {
        this.item = item;
        this.count = count;
    }

    public bool IsEmpty => item == null || count <= 0;

    public bool CanStack(ItemData otherItem)
    {
        return item == otherItem && count < item.maxStackSize;
    }
}

/// <summary>
/// Backpack System (Singleton)
/// </summary>
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Backpack Settings")]
    [Tooltip("Backpack Capacity")]//容量
    public int inventorySize = 24;

    [Tooltip("List of backpack slots")]//列表
    public List<InventorySlot> slots = new List<InventorySlot>();

    [Header("Incident")]
    public System.Action OnInventoryChanged;

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

        InitializeInventory();
    }

    /// <summary>
    /// Initialize bag
    /// </summary>
    void InitializeInventory()
    {
        slots.Clear();
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlot(null, 0));
        }
    }

    /// <summary>
    /// Add item
    /// </summary>
    public bool AddItem(ItemData item, int count = 1)
    {
        if (item == null || count <= 0)
        {
            Debug.LogWarning("Trying to add an invalid item!");
            return false;
        }

        // 1. Try to stack into the existing slots
        foreach (var slot in slots)
        {
            if (slot.item == item && slot.count < item.maxStackSize)
            {
                int spaceLeft = item.maxStackSize - slot.count;
                int amountToAdd = Mathf.Min(count, spaceLeft);

                slot.count += amountToAdd;
                count -= amountToAdd;

                if (count <= 0)
                {
                    OnInventoryChanged?.Invoke();
                    Debug.Log($"Add {item.itemName} x{amountToAdd}（堆叠）");
                    return true;
                }
            }
        }

        // 2. Place in the new slot
        while (count > 0)
        {
            var emptySlot = slots.FirstOrDefault(s => s.IsEmpty);
            if (emptySlot == null)
            {
                Debug.LogWarning("The backpack is full!!！");
                return false;
            }

            int amountToAdd = Mathf.Min(count, item.maxStackSize);
            emptySlot.item = item;
            emptySlot.count = amountToAdd;
            count -= amountToAdd;

            Debug.Log($"Add {item.itemName} x{amountToAdd}（New slot）");
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Remove the item
    /// </summary>
    public bool RemoveItem(ItemData item, int count = 1)
    {
        if (item == null || count <= 0) return false;

        // Check if there is a sufficient quantity
        if (GetItemCount(item) < count)
        {
            Debug.LogWarning($"物品 {item.itemName} 数量不足！");
            return false;
        }

        // Remove
        int remainingToRemove = count;
        foreach (var slot in slots)
        {
            if (slot.item == item && slot.count > 0)
            {
                int amountToRemove = Mathf.Min(slot.count, remainingToRemove);
                slot.count -= amountToRemove;
                remainingToRemove -= amountToRemove;

                if (slot.count <= 0)
                {
                    slot.item = null;
                    slot.count = 0;
                }

                if (remainingToRemove <= 0)
                    break;
            }
        }

        OnInventoryChanged?.Invoke();
        Debug.Log($"Remove {item.itemName} x{count}");
        return true;
    }

    /// <summary>
    /// Obtain the quantity of items
    /// </summary>
    public int GetItemCount(ItemData item)
    {
        if (item == null) return 0;

        return slots.Where(s => s.item == item).Sum(s => s.count);
    }

    /// <summary>
    /// Do you own the item?
    /// </summary>
    public bool HasItem(ItemData item, int count = 1)
    {
        return GetItemCount(item) >= count;
    }

    /// <summary>
    /// Obtain all the seeds
    /// </summary>
    public List<InventorySlot> GetSeeds()
    {
        return slots.Where(s =>
            !s.IsEmpty &&
            s.item.itemType == ItemType.Seed
        ).ToList();
    }

    /// <summary>
    /// Obtain all crops
    /// </summary>
    public List<InventorySlot> GetCrops()
    {
        return slots.Where(s =>
            !s.IsEmpty &&
            s.item.itemType == ItemType.Crop
        ).ToList();
    }

    /// <summary>
    /// Empty the backpack
    /// </summary>
    public void ClearInventory()
    {
        foreach (var slot in slots)
        {
            slot.item = null;
            slot.count = 0;
        }
        OnInventoryChanged?.Invoke();
    }

    /// <summary>
    /// Print the contents of the backpack (for debugging purposes)
    /// </summary>
    public void PrintInventory()
    {
        Debug.Log("===== 背包内容 =====");
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty)
            {
                Debug.Log($"{slot.item.itemName} x{slot.count}");
            }
        }
        Debug.Log("===================");
    }
}
