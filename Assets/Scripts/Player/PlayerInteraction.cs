using UnityEngine;

/// <summary>
/// Player interaction system (planting and harvesting)
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction settings")]
    [Tooltip("Interaction distance")]
    public float interactionDistance = 3f;

    [Tooltip("Interactive buttons")]
    public KeyCode interactKey = KeyCode.E;

    [Tooltip("Backpack button")]
    public KeyCode inventoryKey = KeyCode.Tab;

    [Header("Radiographic inspection")]
    public LayerMask interactableLayer;
    public bool useRaycast = true; // First person uses rays, while third person uses range detection.

    [Header("Current target")]
    public FarmPlot currentPlot;

    [Header("UI reference")]
    public PlantingUI plantingUI;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        // Search for UI
        if (plantingUI == null)
        {
            plantingUI = FindObjectOfType<PlantingUI>();
        }
    }

    void Update()
    {
        DetectInteractable();
        HandleInput();
    }

    /// <summary>
    /// Detect interactive objects
    /// </summary>
    void DetectInteractable()
    {
        currentPlot = null;

        if (useRaycast)
        {
            // 第一人称：射线检测
            Ray ray = mainCamera.ScreenPointToRay(
                new Vector3(Screen.width / 2, Screen.height / 2)
            );
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
            {
                currentPlot = hit.collider.GetComponent<FarmPlot>();
            }
        }
        else
        {
            // 第三人称：范围检测
            Collider[] colliders = Physics.OverlapSphere(
                transform.position,
                interactionDistance,
                interactableLayer
            );

            if (colliders.Length > 0)
            {
                float minDistance = float.MaxValue;
                foreach (var col in colliders)
                {
                    FarmPlot plot = col.GetComponent<FarmPlot>();
                    if (plot != null)
                    {
                        float distance = Vector3.Distance(
                            transform.position,
                            col.transform.position
                        );
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            currentPlot = plot;
                        }
                    }
                }
            }
        }

        UpdateInteractionPrompt();
    }

    /// <summary>
    /// Process input
    /// </summary>
    void HandleInput()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (currentPlot != null)
            {
                InteractWithPlot();
            }
        }

        if (Input.GetKeyDown(inventoryKey))
        {
            ToggleInventory();
        }
    }

    /// <summary>
    /// Interacting with the farm
    /// </summary>
    void InteractWithPlot()
    {
        if (currentPlot.currentState == PlotState.Empty)
        {
            if (plantingUI != null)
            {
                plantingUI.ShowPlantingUI(currentPlot);
            }
            else
            {
                Debug.LogWarning("PlantingUI not found！");
            }
        }
        else if (currentPlot.currentState == PlotState.Mature)
        {
            currentPlot.HarvestCrop();
        }
        else
        {
            Debug.Log(
                string.Format(
                    "During the growth of crops... ({0:F0}%)",
                    currentPlot.growthProgress * 100
                )
            );
        }
    }

    /// <summary>
    /// Update interaction prompt
    /// </summary>
    void UpdateInteractionPrompt()
    {
        if (InteractionPrompt.Instance == null) return;

        if (currentPlot != null)
        {
            string promptText;

            switch (currentPlot.currentState)
            {
                case PlotState.Empty:
                    promptText = string.Format("Key {E} Grow", interactKey);
                    break;

                case PlotState.Mature:
                    promptText = string.Format("Key {V} Reap", interactKey);
                    break;

                case PlotState.Growing:
                    promptText = string.Format(
                        "In Growth  {0:F0}%",
                        currentPlot.growthProgress * 100
                    );
                    break;

                default:
                    promptText = "";
                    break;
            }

            InteractionPrompt.Instance.ShowPrompt(promptText);
        }
        else
        {
            InteractionPrompt.Instance.HidePrompt();
        }
    }

    /// <summary>
    /// Switch backpack display
    /// </summary>
    void ToggleInventory()
    {
        if (InventoryUI.Instance != null)
        {
            InventoryUI.Instance.ToggleInventory();
        }
    }

    /// <summary>
    /// Gizmos Display
    /// </summary>
    void OnDrawGizmos()
    {
        if (useRaycast && mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(
                new Vector3(Screen.width / 2, Screen.height / 2)
            );
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(ray.origin, ray.direction * interactionDistance);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, interactionDistance);
        }
    }
}
