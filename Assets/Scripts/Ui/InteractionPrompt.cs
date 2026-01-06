using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interaction Prompt UI (Singleton)
/// </summary>
public class InteractionPrompt : MonoBehaviour
{
    public static InteractionPrompt Instance { get; private set; }

    [Header("UI component")]
    public GameObject promptPanel;
    public Text promptText;

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
        HidePrompt();
    }

    public void ShowPrompt(string text)
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(true);
        }

        if (promptText != null)
        {
            promptText.text = text;
        }
    }

    public void HidePrompt()
    {
        if (promptPanel != null)
        {
            promptPanel.SetActive(false);
        }
    }
}
