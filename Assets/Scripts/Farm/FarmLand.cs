using UnityEngine;

public class FarmLand : MonoBehaviour
{
    [Header("References")]
    public GameObject currentCrop;
    public PlantingUI plantingUI;
    public Transform player;

    [Header("Settings")]
    public bool isPlowed = true;
    public bool hasCrop = false;
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    private bool playerInRange = false;

    void Start()
    {
        //  强制重置农田状态（确保初始为空）
        hasCrop = false;
        currentCrop = null;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (plantingUI == null)
        {
            plantingUI = FindObjectOfType<PlantingUI>();
        }

        Debug.Log($" {gameObject.name} 初始化: hasCrop={hasCrop}, isPlowed={isPlowed}");
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionDistance)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                Debug.Log($"进入 {gameObject.name} 范围 (hasCrop={hasCrop})");
            }

            //  只有在没有作物时才能打开种植UI
            if (isPlowed && !hasCrop && currentCrop == null && Input.GetKeyDown(interactKey))
            {
                Debug.Log($" 按下E键，打开种植UI");

                if (plantingUI != null)
                {
                    plantingUI.ShowUI(this);
                }
            }
            else if (Input.GetKeyDown(interactKey) && (hasCrop || currentCrop != null))
            {
                Debug.Log($" 农田已有作物，无法重复种植");
            }
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                Debug.Log($" 离开 {gameObject.name} 范围");
            }
        }
    }

    public void PlantCrop(GameObject cropPrefab)
    {
        if (cropPrefab == null)
        {
            Debug.LogError(" 作物预制体为空！");
            return;
        }

        if (!isPlowed)
        {
            Debug.LogWarning(" 土地未耕，无法种植");
            return;
        }

        if (hasCrop || currentCrop != null)
        {
            Debug.LogWarning(" 已有作物，无法重复种植");
            return;
        }

        //  生成作物
        Vector3 cropPosition = transform.position + Vector3.up * 0.1f;
        currentCrop = Instantiate(cropPrefab, cropPosition, Quaternion.identity, transform);
        hasCrop = true;

        Debug.Log($" 作物已种植在 {gameObject.name}");
    }

    public void OnCropHarvested()
    {
        hasCrop = false;
        currentCrop = null;
        Debug.Log($" {gameObject.name} 作物已收割，可重新种植");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}