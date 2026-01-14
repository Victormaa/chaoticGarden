using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI引用")]
    public GameObject inventoryPanel;
    public Transform slotsContainer;
    public GameObject slotPrefab;
    public TextMeshProUGUI titleText;

    [Header("设置")]
    public KeyCode toggleKey = KeyCode.B;
    public int maxSlots = 20;

    private List<InventorySlot> slots = new List<InventorySlot>();
    private bool isOpen = false;

    void Start()
    {
        CreateSlots();

        // 订阅背包变化事件
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += RefreshUI;
        }

        // 初始隐藏
        inventoryPanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInventory();
        }

        if (isOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInventory();
        }
    }

    void CreateSlots()
    {
        // 清除现有格子
        foreach (Transform child in slotsContainer)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();

        // 创建格子
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsContainer);
            InventorySlot slot = slotObj.GetComponent<InventorySlot>();
            if (slot != null)
            {
                slot.ClearSlot();
                slots.Add(slot);
            }
        }
    }

    public void RefreshUI()
    {
        if (InventoryManager.Instance == null) return;

        List<InventoryItem> items = InventoryManager.Instance.GetAllItems();

        // 清空所有格子
        foreach (InventorySlot slot in slots)
        {
            slot.ClearSlot();
        }

        // 填充物品
        for (int i = 0; i < items.Count && i < slots.Count; i++)
        {
            slots[i].SetItem(items[i].itemID, items[i].quantity);
        }

        // 更新标题
        if (titleText != null)
        {
            titleText.text = "背包 (" + items.Count + "/" + maxSlots + ")";
        }
    }

    public void ToggleInventory()
    {
        if (isOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        isOpen = true;
        inventoryPanel.SetActive(true);
        RefreshUI();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseInventory()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);
    }
}