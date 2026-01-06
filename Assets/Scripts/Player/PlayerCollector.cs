using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Player Collection System (3D)
/// </summary>
public class PlayerCollector : MonoBehaviour
{
    [Header("Collection Settings")]
    [Tooltip("Collection radius")]
    public float collectRadius = 2f;

    [Tooltip("Show collection radius")]
    public bool showCollectRadius = true;

    [Header("Current Data")]
    [Tooltip("Total collected value")]
    public int totalCollected = 0;

    [Tooltip("Collected value in current run")]
    public int currentRunCollected = 0;

    [Tooltip("Collected item count")]
    public int itemsCollectedCount = 0;

    [Header("Events")]
    public UnityEvent<int> OnItemCollected;
    public UnityEvent<int> OnTotalUpdated;

    [Header("Audio")]
    public AudioClip collectSound;
    private AudioSource audioSource;

    void Awake()
    {
        // Add audio source
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D sound
    }

    void Start()
    {
        // Initialize events
        if (OnItemCollected == null)
            OnItemCollected = new UnityEvent<int>();

        if (OnTotalUpdated == null)
            OnTotalUpdated = new UnityEvent<int>();
    }

    /// <summary>
    /// Collect item
    /// </summary>
    /// <param name="value">Item value</param>
    /// <param name="position">Collection position (for effects)</param>
    public void CollectItem(int value, Vector3 position)
    {
        // Update data
        totalCollected += value;
        currentRunCollected += value;
        itemsCollectedCount++;

        // Invoke events
        OnItemCollected?.Invoke(value);
        OnTotalUpdated?.Invoke(totalCollected);

        // Play sound
        PlayCollectSound();

        // Spawn floating text
        SpawnFloatingText(value, position);

        Debug.Log(
            string.Format(
                "Collected ${0}, Total: ${1}",
                value,
                totalCollected
            )
        );
    }

    /// <summary>
    /// Play collect sound
    /// </summary>
    void PlayCollectSound()
    {
        if (collectSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Random pitch
            audioSource.PlayOneShot(collectSound);
        }
    }

    /// <summary>
    /// Spawn floating value text
    /// </summary>
    void SpawnFloatingText(int value, Vector3 position)
    {
        GameObject textObj = new GameObject("FloatingValue");
        textObj.transform.position = position + Vector3.up * 0.5f;

        TextMesh tm = textObj.AddComponent<TextMesh>();
        tm.text = string.Format("+${0}", value);
        tm.fontSize = 60;
        tm.color = Color.yellow;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;

        // Add billboard component (if exists)
        textObj.AddComponent<Billboard>();

        // Animation: rise and fade out
        StartCoroutine(FloatingTextAnimation(textObj, tm));
    }

    /// <summary>
    /// Floating text animation
    /// </summary>
    System.Collections.IEnumerator FloatingTextAnimation(
        GameObject textObj,
        TextMesh tm
    )
    {
        Vector3 startPos = textObj.transform.position;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // Rise
            textObj.transform.position =
                startPos + Vector3.up * t * 2f;

            // Fade out
            Color color = tm.color;
            color.a = 1f - t;
            tm.color = color;

            yield return null;
        }

        Destroy(textObj);
    }

    /// <summary>
    /// Reset current run data
    /// </summary>
    public void ResetRun()
    {
        currentRunCollected = 0;
        itemsCollectedCount = 0;
    }

    /// <summary>
    /// Get total collected value
    /// </summary>
    public int GetTotalCollected()
    {
        return totalCollected;
    }

    /// <summary>
    /// Gizmos: show collection radius
    /// </summary>
    void OnDrawGizmos()
    {
        if (showCollectRadius)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
            Gizmos.DrawWireSphere(transform.position, collectRadius);
        }
    }
}
