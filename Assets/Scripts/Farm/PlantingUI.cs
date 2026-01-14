using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlantingUI : MonoBehaviour
{
    [Header("UI References")]//引用
    public GameObject uiPanel;
    public Transform itemContainer;
    public GameObject itemButtonPrefab;

    [Header("Available Plantable Items List")]//可种植物品列表
    public List<CropData> availableCrops = new List<CropData>();

    [Header("Particle Light Effect Settings")]
    public GameObject selectionParticlePrefab; // Particle 

    private FarmLand currentFarmLand;
    private List<GameObject> spawnedButtons = new List<GameObject>();
    private List<CropData> availablePlantList = new List<CropData>();
    private List<GameObject> spawnedParticles = new List<GameObject>(); // 存储粒子

    private int currentSelectedIndex = 0;
    private bool isUIActive = false;

    void Start()
    {
        if (uiPanel == null) Debug.LogError("PlantingUI: UI Panel not assigned!");
        if (itemContainer == null) Debug.LogError("PlantingUI: Item Container not assigned!");
        if (itemButtonPrefab == null) Debug.LogError("PlantingUI: Item Button Prefab not assigned!");
        if (selectionParticlePrefab == null) Debug.LogWarning("PlantingUI: Selection Particle Prefab not assigned!");

        HideUI();
    }

    void Update()
    {
        if (!isUIActive || availablePlantList.Count == 0) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentSelectedIndex = (currentSelectedIndex + 1) % availablePlantList.Count;
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentSelectedIndex--;
            if (currentSelectedIndex < 0)
                currentSelectedIndex = availablePlantList.Count - 1;
            UpdateSelection();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            PlantSelectedCrop();
        }
    }

    public void ShowUI(FarmLand farmLand)
    {
        currentFarmLand = farmLand;

        availablePlantList.Clear();
        foreach (var crop in availableCrops)
        {
            if (crop != null)
            {
                availablePlantList.Add(crop);
            }
        }

        if (availablePlantList.Count == 0)
        {
            Debug.LogWarning("No plantable crops available!");//没有可种植的作物
            return;
        }

        ClearButtons();
        SpawnButtons();

        currentSelectedIndex = 0;
        UpdateSelection();

        uiPanel.SetActive(true);
        isUIActive = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideUI()
    {
        uiPanel.SetActive(false);
        isUIActive = false;
        ClearButtons();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ClearButtons()
    {
        // Clear 按钮
        foreach (var btn in spawnedButtons)
        {
            if (btn != null)
                Destroy(btn);
        }
        spawnedButtons.Clear();

        // Clear 粒子
        foreach (var particle in spawnedParticles)
        {
            if (particle != null)
                Destroy(particle);
        }
        spawnedParticles.Clear();
    }

    void SpawnButtons()
    {
        for (int i = 0; i < availablePlantList.Count; i++)
        {
            CropData crop = availablePlantList[i];

            GameObject btnObj = Instantiate(itemButtonPrefab, itemContainer);
            spawnedButtons.Add(btnObj);

            // Set icon
            Transform iconTransform = btnObj.transform.Find("Icon");
            if (iconTransform != null && crop.icon != null)
            {
                Image iconImage = iconTransform.GetComponent<Image>();
                if (iconImage != null)
                {
                    iconImage.sprite = crop.icon;
                }
            }

            // Set name
            Transform nameTransform = btnObj.transform.Find("Name");
            if (nameTransform != null)
            {
                TextMeshProUGUI nameText = nameTransform.GetComponent<TextMeshProUGUI>();
                if (nameText != null)
                {
                    nameText.text = crop.cropName;
                }
            }

            // Create particle system (but hide it initially)
            if (selectionParticlePrefab != null)
            {
                GameObject particleObj = Instantiate(selectionParticlePrefab, btnObj.transform);
                particleObj.transform.localPosition = Vector3.zero;
                particleObj.SetActive(false); // Hidden by default
                spawnedParticles.Add(particleObj);
            }
            else
            {
                spawnedParticles.Add(null);
            }
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < spawnedButtons.Count; i++)
        {
            GameObject btn = spawnedButtons[i];
            if (btn == null) continue;

            RectTransform rectTransform = btn.GetComponent<RectTransform>();

            if (i == currentSelectedIndex)
            {
                // Selected state: show particle, scale up
                if (rectTransform != null)
                {
                    rectTransform.localScale = Vector3.one * 1.15f;
                }

                // Show particle
                if (i < spawnedParticles.Count && spawnedParticles[i] != null)
                {
                    spawnedParticles[i].SetActive(true);
                }
            }
            else
            {
                // Unselected state: hide particle, normal size
                if (rectTransform != null)
                {
                    rectTransform.localScale = Vector3.one;
                }

                // Hide particle
                if (i < spawnedParticles.Count && spawnedParticles[i] != null)
                {
                    spawnedParticles[i].SetActive(false);
                }
            }
        }

        Debug.Log("Currently selected: " + availablePlantList[currentSelectedIndex].cropName);
    }

    void PlantSelectedCrop()
    {
        if (currentFarmLand == null)
        {
            Debug.LogWarning("No farmland currently selected");//当前没有选中的土地
            return;
        }

        if (currentSelectedIndex < 0 || currentSelectedIndex >= availablePlantList.Count)
        {
            Debug.LogWarning("Selected index out of bounds");//选中索引越界
            return;
        }

        CropData selectedCrop = availablePlantList[currentSelectedIndex];

        if (selectedCrop == null)
        {
            Debug.LogWarning("Selected crop data is null");//选中的作物数据为空
            return;
        }

        currentFarmLand.PlantCrop(selectedCrop);
        HideUI();

        Debug.Log("Planted: " + selectedCrop.cropName);
    }
}