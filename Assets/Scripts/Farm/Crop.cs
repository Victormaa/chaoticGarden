using UnityEngine;
using System.Collections;

public class Crop : MonoBehaviour
{
    [Header("作物数据")]
    public CropData cropData;

    [Header("生长状态")]
    public int currentStage = 0;
    public float growthTimer = 0f;
    public bool isFullyGrown = false;

    [Header("收割设置")]
    public KeyCode harvestKey = KeyCode.C;
    public float harvestDistance = 3f;

    [Header("掉落物设置")]
    public GameObject collectiblePrefab;
    public int dropCount = 3;
    public float minForce = 5f;
    public float maxForce = 10f;

    private Transform player;
    private FarmLand farmLand;
    private bool playerInRange = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        farmLand = GetComponentInParent<FarmLand>();

        if (cropData == null)
        {
            Debug.LogError("CropData 未分配！");
            enabled = false;
            return;
        }

        UpdateAppearance();
        Debug.Log($"种植了 {cropData.cropName}，需要 {cropData.totalGrowthTime} 秒成熟");
    }

    void Update()
    {
        // 生长逻辑
        if (!isFullyGrown)
        {
            growthTimer += Time.deltaTime;
            float progress = growthTimer / cropData.totalGrowthTime;
            int newStage = Mathf.FloorToInt(progress * cropData.growthStages.Length);
            newStage = Mathf.Min(newStage, cropData.growthStages.Length - 1);

            if (newStage != currentStage)
            {
                currentStage = newStage;
                UpdateAppearance();
            }

            if (progress >= 1f)
            {
                isFullyGrown = true;
                Debug.Log($"{cropData.cropName} 已成熟，可以收割");
            }
        }

        // 收割检测
        if (isFullyGrown && player != null)
        {
            float distance = Vector3.Distance(player.position, transform.position);
            if (distance <= harvestDistance)
            {
                if (!playerInRange)
                {
                    playerInRange = true;
                    Debug.Log($"按 {harvestKey} 键收割 {cropData.cropName}");
                }

                if (Input.GetKeyDown(harvestKey))
                {
                    Harvest();
                }
            }
            else
            {
                playerInRange = false;
            }
        }
    }

    protected virtual void Harvest()
    {
        Debug.Log($"收割 {cropData.cropName}");

        // 添加到背包
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(cropData.cropID, cropData.harvestAmount);
            Debug.Log($"获得 {cropData.cropName} x{cropData.harvestAmount}");
        }

        // 生成掉落物
        if (collectiblePrefab != null)
        {
            StartCoroutine(SpawnCollectibles());
        }
        else
        {
            CompletHarvest();
        }
    }

    IEnumerator SpawnCollectibles()
    {
        for (int i = 0; i < dropCount; i++)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            GameObject collectible = Instantiate(collectiblePrefab, spawnPos, Quaternion.identity);

            Collectible comp = collectible.GetComponent<Collectible>();
            if (comp != null)
            {
                comp.itemID = cropData.cropID;
                comp.amount = 1;
            }

            Rigidbody rb = collectible.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDir = new Vector3(
                    Random.Range(-0.05f, 0.05f),
                    Random.Range(0.8f, 1.2f),
                    Random.Range(-0.05f, 0.05f)
                ).normalized;

                float force = Random.Range(minForce, maxForce);
                rb.AddForce(randomDir * force, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(0.3f);
        CompletHarvest();
    }

    void CompletHarvest()
    {
        if (farmLand != null)
        {
            farmLand.OnCropHarvested();
        }

        Destroy(gameObject);
    }

    void UpdateAppearance()
    {
        if (cropData.growthStages == null || cropData.growthStages.Length == 0)
        {
            return;
        }

        if (currentStage >= cropData.growthStages.Length)
        {
            currentStage = cropData.growthStages.Length - 1;
        }

        CropGrowthStage stage = cropData.growthStages[currentStage];

        if (stage.model != null)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            //GameObject model = Instantiate(stage.model, transform.position, Quaternion.identity, transform);
            //model.transform.localScale = stage.scale;
        }
    }
}