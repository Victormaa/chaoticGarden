using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Inventory UI (Singleton)
/// </summary>
public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("UI Components")]
    public GameObject inventoryPanel;
    public Transform slotContainer;
    public GameObject slotPrefab;

    [Header("Detail Panel")]
    public GameObject detailPanel;
    public Image detailIcon;
    public Text detailName;
    public Text detailDescription;
    public Text detailCount;
    public Text detailType;

    [Header("Category Filters")]
    public Toggle showAllToggle;
    public Toggle showSeedsToggle;
    public Toggle showCropsToggle;

    private List<GameObject> slotObjects = new List<GameObject>();
    private ItemType currentFilter = ItemType.Seed; // Default: show all

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        inventoryPanel.SetActive(false);

        // Subscribe to inventory change event
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged += RefreshInventoryUI;
        }

        // Toggle events
        if (showAllToggle != null)
            showAllToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetFilter(ItemType.Other); });

        if (showSeedsToggle != null)
            showSeedsToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetFilter(ItemType.Seed); });

        if (showCropsToggle != null)
            showCropsToggle.onValueChanged.AddListener((isOn) => { if (isOn) SetFilter(ItemType.Crop); });
    }

    /// <summary>
    /// Toggle inventory visibility
    /// </summary>
    public void ToggleInventory()
    {
        bool newState = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(newState);

        if (newState)
        {
            RefreshInventoryUI();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Set filter
    /// </summary>
    void SetFilter(ItemType filter)
    {
        currentFilter = filter;
        RefreshInventoryUI();
    }

    /// <summary>
    /// Refresh inventory UI
    /// </summary>
    public void RefreshInventoryUI()
    {
        if (!inventoryPanel.activeSelf) return;

        // Clear old slots
        foreach (var obj in slotObjects)
        {
            Destroy(obj);
        }
        slotObjects.Clear();

        // Get inventory data
        List<InventorySlot> slots = InventorySystem.Instance.slots;

        foreach (var slot in slots)
        {
            if (slot.IsEmpty) continue;

            // Filtering
            if (currentFilter != ItemType.Other && slot.item.itemType != currentFilter)
                continue;

            CreateSlotUI(slot);
        }
    }

    /// <summary>
    /// Create slot UI
    /// </summary>
    void CreateSlotUI(InventorySlot slot)
    {
        GameObject slotObj = Instantiate(slotPrefab, slotContainer);
        slotObjects.Add(slotObj);

        // Set icon
        Image icon = slotObj.transform.Find("Icon")?.GetComponent<Image>();
        if (icon != null)
        {
            if (slot.item.icon != null)
            {
                icon.sprite = slot.item.icon;
                icon.color = Color.white;
            }
            else
            {
                icon.color = Color.gray;
            }
        }

        // Set count
        Text countText = slotObj.transform.Find("Count")?.GetComponent<Text>();
        if (countText != null)
        {
            countText.text = slot.count > 1 ? slot.count.ToString() : "";
        }

        // Set name
        Text nameText = slotObj.transform.Find("Name")?.GetComponent<Text>();
        if (nameText != null)
        {
            nameText.text = slot.item.itemName;
        }

        // Add click event
        Button button = slotObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnSlotClicked(slot));
        }
    }

    /// <summary>
    /// Slot click event
    /// </summary>
    void OnSlotClicked(InventorySlot slot)
    {
        ShowItemDetail(slot);
    }

    /// <summary>
    /// Show item details
    /// </summary>
    void ShowItemDetail(InventorySlot slot)
    {
        if (detailPanel == null) return;

        detailPanel.SetActive(true);

        if (detailIcon != null && slot.item.icon != null)
        {
            detailIcon.sprite = slot.item.icon;
        }

        if (detailName != null)
        {
            detailName.text = slot.item.itemName;
        }

        if (detailDescription != null)
        {
            detailDescription.text = slot.item.description;
        }

        if (detailCount != null)
        {
            detailCount.text = string.Format("Count: {0}", slot.count);
        }

        if (detailType != null)
        {
            string typeText;

            switch (slot.item.itemType)
            {
                case ItemType.Seed:
                    typeText = "Seed";
                    break;
                case ItemType.Crop:
                    typeText = "Crop";
                    break;
                case ItemType.Tool:
                    typeText = "Tool";
                    break;
                default:
                    typeText = "Other";
                    break;
            }

            detailType.text = string.Format("Type: {0}", typeText);

            // Show extra info
            if (slot.item is SeedData seed)
            {
                detailType.text += string.Format("\nGrow Time: {0} sec", seed.growthTime);
                detailType.text += string.Format("\nYield: {0}", seed.harvestYield);
            }
            else if (slot.item is CropData crop)
            {
                detailType.text += string.Format("\nValue: ${0}", crop.cropValue);
                detailType.text += string.Format("\nQuality: {0} star(s)", crop.quality);
            }
        }
    }

    /// <summary>
    /// Close inventory
    /// </summary>
    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnDestroy()
    {
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.OnInventoryChanged -= RefreshInventoryUI;
        }
    }
}
