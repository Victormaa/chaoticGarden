using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

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

        if (plantButton != null)
        {
            plantButton.onClick.AddListener(OnPlantButtonClicked);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }

        CreateCropButtons();
    }

    void CreateCropButtons()
    {
        if (cropButtonContainer == null || cropButtonPrefab == null)
        {
            Debug.LogWarning("PlantingUI: 缺少必要的引用");
            return;
        }

        foreach (GameObject btn in cropButtons)
        {
            Destroy(btn);
        }
        cropButtons.Clear();

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
                button.onClick.AddListener(() => OnCropButtonClicked(cropData));
            }
        }
    }

    void OnCropButtonClicked(CropData crop)
    {
        selectedCrop = crop;
        Debug.Log($"选择了作物: {crop.cropName}");

        int index = availableCrops.IndexOf(crop);
        if (index >= 0 && index < cropButtons.Count)
        {
            // 重置所有按钮颜色
            foreach (GameObject btn in cropButtons)
            {
                Image bg = btn.GetComponent<Image>();
                if (bg != null)
                {
                    bg.color = Color.white;
                }
            }

            // 高亮选中的按钮
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
                           $"生长时间: {crop.totalGrowthTime} 秒\n" +
                           $"产出: {crop.harvestAmount} 个";
    }

    public void ShowUI(FarmLand farmLand)
    {
        Debug.Log(" ShowUI 被调用");

        if (farmLand == null)
        {
            Debug.LogWarning("farmLand 为空");
            return;
        }

        if (plantingPanel == null)
        {
            Debug.LogError(" plantingPanel 未赋值！");
            return;
        }

        // ✅ 每次打开UI时重置种植标志
        isPlanting = false;

        currentFarmLand = farmLand;
        plantingPanel.SetActive(true);

        Debug.Log(" UI Panel 已激活");

        if (titleText != null)
        {
            titleText.text = "选择要种植的作物";
        }

        selectedCrop = null;

        SetPlayerControlsEnabled(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        RefreshCropButtons();
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

        //  关闭UI时重置种植标志
        isPlanting = false;

        Debug.Log(" 种植UI已关闭，种植标志已重置");
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

    public void OnPlantButtonClicked()
    {
        //  防止重复点击
        if (isPlanting)
        {
            Debug.LogWarning(" 正在种植中，请稍后...");
            return;
        }

        if (currentFarmLand == null)
        {
            Debug.LogWarning("❌ 未选择农田");
            return;
        }

        if (selectedCrop == null)
        {
            Debug.LogWarning("❌ 请先选择作物");
            return;
        }

        //  检查农田是否已有作物
        if (currentFarmLand.hasCrop || currentFarmLand.currentCrop != null)
        {
            Debug.LogWarning("❌ 该农田已经有作物了");
            HideUI();
            return;
        }

        //  标记为正在种植
        isPlanting = true;

        Debug.Log($" 开始种植 {selectedCrop.cropName}");

        // 执行种植
        if (currentFarmLand != null)
        {
            currentFarmLand.PlantCrop(selectedCrop.cropPrefab);
        }
        // 先关闭UI
        HideUI();

        Debug.Log($"成功种植 {selectedCrop.cropName}");

        // 0.1秒后重置标志（防止快速重复点击）
        StartCoroutine(ResetPlantingFlag());
    }

    IEnumerator ResetPlantingFlag()
    {
        yield return new WaitForSeconds(0.1f);
        isPlanting = false;
        Debug.Log(" 种植标志已重置");
    }

    void OnCancelButtonClicked()
    {
        Debug.Log(" 取消种植");
        HideUI();
    }
}