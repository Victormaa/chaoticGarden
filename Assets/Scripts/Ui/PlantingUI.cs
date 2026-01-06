using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Planting UI (Selecting Seeds)
/// </summary>
public class PlantingUI : MonoBehaviour
{
    [Header("UI component")]
    public GameObject plantingPanel;
    public Transform seedButtonContainer;
    public GameObject seedButtonPrefab;

    [Header("Current farmland")]
    private FarmPlot targetPlot;

    private List<GameObject> seedButtons = new List<GameObject>();

    void Start()
    {
        HidePlantingUI();
    }

    /// <summary>
    /// Display planting UI
    /// </summary>
    public void ShowPlantingUI(FarmPlot plot)
    {
        targetPlot = plot;
        plantingPanel.SetActive(true);

        // Lock the mouse
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        RefreshSeedList();
    }

    /// <summary>
    /// Hidden planting UI
    /// </summary>
    public void HidePlantingUI()
    {
        plantingPanel.SetActive(false);
        targetPlot = null;

        // Restore mouse lock
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Refresh the list of seeds
    /// </summary>
    void RefreshSeedList()
    {
        // Clear the old buttons
        foreach (var btn in seedButtons)
        {
            Destroy(btn);
        }
        seedButtons.Clear();

        // Retrieve all the seeds from the backpack
        List<InventorySlot> seeds = InventorySystem.Instance.GetSeeds();

        if (seeds.Count == 0)
        {
            // No seed indication
            CreateNoSeedsMessage();
            return;
        }

        // Create a button for each seed
        foreach (var slot in seeds)
        {
            CreateSeedButton(slot);
        }
    }

    /// <summary>
    /// Create Seed Button
    /// </summary>
    void CreateSeedButton(InventorySlot slot)
    {
        SeedData seed = slot.item as SeedData;
        if (seed == null) return;

        GameObject buttonObj = Instantiate(seedButtonPrefab, seedButtonContainer);
        seedButtons.Add(buttonObj);

        // settings picture
        Image iconImage = buttonObj.transform.Find("Icon")?.GetComponent<Image>();
        if (iconImage != null && seed.icon != null)
        {
            iconImage.sprite = seed.icon;
        }

        // Setting Name
        Text nameText = buttonObj.transform.Find("Name")?.GetComponent<Text>();
        if (nameText != null)
        {
            nameText.text = seed.itemName;
        }

        // Setting Number
        Text countText = buttonObj.transform.Find("Count")?.GetComponent<Text>();
        if (countText != null)
        {
            countText.text = string.Format("x{0}", slot.count);
        }

        // Setting time
        Text timeText = buttonObj.transform.Find("GrowTime")?.GetComponent<Text>();
        if (timeText != null)
        {
            timeText.text = string.Format("{0}√Î", seed.growthTime);
        }

        // Add new item
        Button button = buttonObj.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnSeedButtonClicked(seed));
        }
    }

    /// <summary>
    /// Create the "No Seeds" prompt Ui
    /// </summary>
    void CreateNoSeedsMessage()
    {
        GameObject messageObj = new GameObject("NoSeedsMessage");
        messageObj.transform.SetParent(seedButtonContainer);

        Text text = messageObj.AddComponent<Text>();
        text.text = "There are no seeds in the backpack!";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 20;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        RectTransform rt = messageObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 50);

        seedButtons.Add(messageObj);
    }

    /// <summary>
    /// Seed button click event
    /// </summary>
    void OnSeedButtonClicked(SeedData seed)
    {
        if (targetPlot == null)
        {
            Debug.LogWarning("The target farm has been lost!");
            HidePlantingUI();
            return;
        }

        // Plant seeds
        bool success = targetPlot.PlantSeed(seed);

        if (success)
        {
            // Remove the seeds from the backpack
            InventorySystem.Instance.RemoveItem(seed, 1);

            Debug.Log(string.Format("plant {0}", seed.itemName));

            // Close UI
            HidePlantingUI();
        }
    }

    /// <summary>
    /// Close button
    /// </summary>
    public void OnCloseButtonClicked()
    {
        HidePlantingUI();
    }
}
