using UnityEngine;

public class FarmLand : MonoBehaviour
{
    [Header("Land Status")]//farm状态
    public bool isOccupied = false;

    [Header("References")]
    public InventoryManager inventory;
    public PlantingUI plantingUI;
    public Transform plantPosition;
    public GameObject commonSeedPrefab;

    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.F;

    private Transform player;
    private Crop currentCrop;
    private bool playerInRange = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        if (inventory == null)
        {
            inventory = FindObjectOfType<InventoryManager>();
        }

        if (plantingUI == null)
        {
            plantingUI = FindObjectOfType<PlantingUI>();
        }

        if (plantPosition == null)
        {
            plantPosition = transform;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= interactionDistance;

        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            if (!isOccupied)
            {
                OnPlayerEnter();
            }
            else if (currentCrop != null)
            {
                HarvestCrop();
            }
        }
    }

    public void OnPlayerEnter()
    {
        if (isOccupied)
        {
            Debug.Log("Land is already occupied");
            return;
        }

        if (plantingUI != null)
        {
            plantingUI.ShowUI(this);
        }
        else
        {
            Debug.LogError("PlantingUI not found!");
        }
    }

    public void PlantCrop(CropData cropData)
    {
        if (isOccupied)
        {
            Debug.Log("Land is already occupied");
            return;
        }

        if (cropData == null)
        {
            Debug.LogError("CropData is null!");
            return;
        }

        GameObject cropObj = new GameObject("Crop_" + cropData.cropName);
        cropObj.transform.position = plantPosition.position;
        cropObj.transform.rotation = plantPosition.rotation;
        cropObj.transform.SetParent(transform);

        currentCrop = cropObj.AddComponent<Crop>();
        currentCrop.Initialize(cropData, commonSeedPrefab, this);

        isOccupied = true;

        if (plantingUI != null)
        {
            plantingUI.HideUI();
        }

        Debug.Log("Successfully planted " + cropData.cropName);
    }

    public void HarvestCrop()
    {
        if (currentCrop != null)
        {
            currentCrop.Harvest();
            currentCrop = null;
            isOccupied = false;
            Debug.Log("Harvest completed");//收获完成
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}