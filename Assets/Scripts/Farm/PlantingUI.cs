using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlantingUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject plantingPanel;
    public TMP_Text titleText;
    public Transform cropButtonContainer;
    public GameObject cropButtonPrefab;
    public Button plantButton;
    public Button cancelButton;
    public TMP_Text cropInfoText;

    [Header("Player References")]
    public MonoBehaviour playerController;
    public MonoBehaviour cameraController;

    [Header("Game Data")]
    public List<CropData> availableCrops;

    private List<GameObject> cropButtons = new List<GameObject>();
    public CropData selectedCrop;
    public FarmLand currentFarmLand;
    private bool isPlanting = false;

    void Awake()
    {
        if (plantingPanel != null)
        {
            plantingPanel.SetActive(false);
        }

        // ✅ 移除旧监听器，防止重复
        if (plantButton != null)
        {
            plantButton.onClick.RemoveAllListeners();
            plantButton.onClick.AddListener(OnPlantButtonClicked);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }

        CreateCropButtons();

        Debug.Log("✅ PlantingUI 初始化完成");
    }

    void CreateCropButtons()
    {
        if (cropButtonContainer == null || cropButtonPrefab == null)
        {
            Debug.LogWarning("PlantingUI: 缺少必要的引用");
            return;
        }

        // 清理旧按钮
        foreach (GameObject btn in cropButtons)
        {
            if (btn != null) Destroy(btn);
        }
        cropButtons.Clear();

        // 创建新按钮
        foreach (CropData crop in availableCrops)
        {
            if (crop == null) continue;

            GameObject buttonObj = Instantiate(cropButtonPrefab, cropButtonContainer);
            cropButtons.Add(buttonObj);

            Image icon = buttonObj.transform.Find("Icon")?.GetComponent<Image>();
            if (icon != null && crop.icon != null)
            {
                icon.sprite = crop.icon;
            }

            TextMeshProUGUI nameText = buttonObj.transform.Find("Name")?.GetComponent<TextMeshProUGUI>();
            if (nameText != null)
            {
                nameText.text = crop.cropName;
            }

            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                CropData cropData = crop;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnCropButtonClicked(cropData));
            }
        }
    }

    void OnCropButtonClicked(CropData crop)
    {
        selectedCrop = crop;
        Debug.Log($"Selected crop: {crop.cropName}");

        // 重置所有按钮颜色
        foreach (GameObject btn in cropButtons)
        {
            Image bg = btn.GetComponent<Image>();
            if (bg != null) bg.color = Color.white;
        }

        // 高亮选中的按钮
        int index = availableCrops.IndexOf(crop);
        if (index >= 0 && index < cropButtons.Count)
        {
            Image selectedBg = cropButtons[index].GetComponent<Image>();
            if (selectedBg != null)
            {
                selectedBg.color = new Color(0.7f, 1f, 0.7f);
            }
        }

        UpdateCropInfo(crop);
    }

    void UpdateCropInfo(CropData crop)
    {
        if (cropInfoText == null) return;

        cropInfoText.text = $"<b>{crop.cropName}</b>\n" +
                           $"Growth Time: {crop.totalGrowthTime}s\n" +
                           $"Harvest: {crop.harvestAmount}";
    }

    public void ShowUI(FarmLand farmLand)
    {
        if (farmLand == null || plantingPanel == null)
        {
            Debug.LogWarning("FarmLand or PlantingPanel is null");
            return;
        }

        // ✅ 重置种植标志
        isPlanting = false;

        currentFarmLand = farmLand;
        plantingPanel.SetActive(true);

        if (titleText != null)
        {
            titleText.text = "Select a crop to plant";
        }

        selectedCrop = null;

        SetPlayerControlsEnabled(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        RefreshCropButtons();

        Debug.Log("✅ Planting UI opened");
    }

    public void HideUI()
    {
        if (plantingPanel != null)
        {
            plantingPanel.SetActive(false);
        }

        currentFarmLand = null;

        SetPlayerControlsEnabled(true);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // ✅ 重置种植标志
        isPlanting = false;

        Debug.Log("✅ Planting UI closed");
    }

    void SetPlayerControlsEnabled(bool enabled)
    {
        if (playerController != null)
        {
            playerController.enabled = enabled;
        }

        if (cameraController != null)
        {
            cameraController.enabled = enabled;
        }
    }

    void RefreshCropButtons()
    {
        if (cropButtonContainer == null) return;

        foreach (Transform child in cropButtonContainer)
        {
            Destroy(child.gameObject);
        }
        cropButtons.Clear();

        CreateCropButtons();
    }

    // ✅ 核心方法：防止重复种植
    public void OnPlantButtonClicked()
    {
        // 1. 防止重复点击
        if (isPlanting)
        {
            Debug.LogWarning("⚠️ Planting in progress, please wait...");
            return;
        }

        // 2. 立即标记为正在种植
        isPlanting = true;

        // 3. 参数检查
        if (currentFarmLand == null)
        {
            Debug.LogWarning("❌ No farmland selected");
            isPlanting = false;
            return;
        }

        if (selectedCrop == null)
        {
            Debug.LogWarning("❌ Please select a crop first");
            isPlanting = false;
            return;
        }

        if (selectedCrop.cropPrefab == null)
        {
            Debug.LogError($"❌ {selectedCrop.cropName} has no prefab!");
            isPlanting = false;
            return;
        }

        // 4. 检查农田状态
        if (currentFarmLand.hasCrop || currentFarmLand.currentCrop != null)
        {
            Debug.LogWarning("❌ Farmland already has a crop");
            isPlanting = false;
            HideUI();
            return;
        }

        Debug.Log($"🌱 Planting {selectedCrop.cropName}");

        // 5. 执行种植
        currentFarmLand.PlantCrop(selectedCrop.cropPrefab);

        Debug.Log($"✅ {selectedCrop.cropName} planted successfully");

        // 6. 关闭UI
        HideUI();
    }

    void OnCancelButtonClicked()
    {
        Debug.Log("❌ Planting cancelled");
        HideUI();
    }
}