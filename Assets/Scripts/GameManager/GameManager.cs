using UnityEngine;

/// <summary>
/// Game Manager (Endless Farming Game)
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public GameState currentState = GameState.Playing;

    [Header("Player Reference")]
    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    private GameObject playerInstance;

    [Header("Crop Spawning")]
    public GameObject[] cropPrefabs;
    public int initialCropCount = 5;
    public float spawnRadius = 15f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitializeGame();
    }

    void Update()
    {
        // ESC key to pause / resume
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    /// <summary>
    /// Initialize game (single endless session)
    /// </summary>
    void InitializeGame()
    {
        SpawnPlayer();
        SpawnInitialCrops();

        if (GameUI.Instance != null)
        {
            GameUI.Instance.ShowInstruction(
                "Press SPACE to harvest mature crops",
                5f
            );
        }
    }

    /// <summary>
    /// Spawn player
    /// </summary>
    void SpawnPlayer()
    {
        if (playerInstance != null) return;

        Vector3 spawnPos = playerSpawnPoint != null
            ? playerSpawnPoint.position
            : Vector3.zero;

        if (playerPrefab != null)
        {
            playerInstance = Instantiate(
                playerPrefab,
                spawnPos,
                Quaternion.identity
            );
        }
        else
        {
            playerInstance = GameObject.FindGameObjectWithTag("Player");
        }

        SetupCamera();
    }

    /// <summary>
    /// Setup camera follow
    /// </summary>
    void SetupCamera()
    {
        CameraController camController = FindObjectOfType<CameraController>();
        if (camController != null &&
            camController.virtualCamera != null &&
            playerInstance != null)
        {
            camController.virtualCamera.Follow = playerInstance.transform;
        }

        if (camController == null)
        {
            Cinemachine.CinemachineVirtualCamera vcam =
                FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();

            if (vcam != null && playerInstance != null)
            {
                vcam.Follow = playerInstance.transform;
            }
        }
    }

    /// <summary>
    /// Spawn initial crops
    /// </summary>
    void SpawnInitialCrops()
    {
        if (cropPrefabs == null || cropPrefabs.Length == 0)
        {
            Debug.LogWarning(
                "No crop prefabs assigned! Please add crop prefabs in GameManager."
            );
            return;
        }

        for (int i = 0; i < initialCropCount; i++)
        {
            Vector2 randomCircle =
                Random.insideUnitCircle * spawnRadius;

            Vector3 spawnPos =
                new Vector3(randomCircle.x, 0, randomCircle.y);

            GameObject cropPrefab =
                cropPrefabs[Random.Range(0, cropPrefabs.Length)];

            GameObject cropInstance =
                Instantiate(cropPrefab, spawnPos, Quaternion.identity);

            cropInstance.name =
                cropPrefab.name + "_" + i;
        }

        Debug.Log(
            string.Format(
                "Spawned {0} crops",
                initialCropCount
            )
        );
    }

    /// <summary>
    /// Toggle pause
    /// </summary>
    void TogglePause()
    {
        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            Time.timeScale = 0f;

            if (GameUI.Instance != null)
            {
                GameUI.Instance.ShowInstruction(
                    "Game Paused - Press ESC to resume"
                );
            }
        }
        else if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            Time.timeScale = 1f;

            if (GameUI.Instance != null)
            {
                GameUI.Instance.ShowInstruction("");
            }
        }
    }
}

/// <summary>
/// Game state enum (no level completion)
/// </summary>
public enum GameState
{
    Playing,
    Paused,
    GameOver
}
