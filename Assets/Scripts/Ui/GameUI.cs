using UnityEngine;
using TMPro;

/// <summary>
/// Game UI Manager
/// </summary>
public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    [Header("UI Reference")]
    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI itemCountText;
    public TextMeshProUGUI instructionText;

    [Header("Currency Animation")]
    public float currencyAnimDuration = 0.3f;
    private int displayedCurrency = 0;
    private int targetCurrency = 0;

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
        UpdateUI();
    }

    void Update()
    {
        // Smooth animated currency value
        if (displayedCurrency != targetCurrency)
        {
            displayedCurrency = Mathf.RoundToInt(
                Mathf.Lerp(
                    displayedCurrency,
                    targetCurrency,
                    Time.deltaTime / currencyAnimDuration
                )
            );

            if (currencyText != null)
            {
                currencyText.text = string.Format("${0}", displayedCurrency);
            }
        }
    }

    /// <summary>
    /// Update currency display
    /// </summary>
    public void UpdateCurrency(int amount)
    {
        targetCurrency = amount;
    }

    /// <summary>
    /// Update item count
    /// </summary>
    public void UpdateItemCount(int count)
    {
        if (itemCountText != null)
        {
            itemCountText.text = string.Format("Items: {0}", count);
        }
    }

    /// <summary>
    /// Show instruction text
    /// </summary>
    public void ShowInstruction(string text, float duration = 3f)
    {
        if (instructionText != null)
        {
            instructionText.text = text;
            instructionText.gameObject.SetActive(true);

            CancelInvoke(nameof(HideInstruction));
            Invoke(nameof(HideInstruction), duration);
        }
    }

    void HideInstruction()
    {
        if (instructionText != null)
        {
            instructionText.gameObject.SetActive(false);
        }
    }

    void UpdateUI()
    {
        if (currencyText != null)
        {
            currencyText.text = "$0";
        }

        if (itemCountText != null)
        {
            itemCountText.text = "Items: 0";
        }
    }
}
