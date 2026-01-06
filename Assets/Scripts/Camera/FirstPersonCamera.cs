using UnityEngine;

/// <summary>
/// First Person Camera Controller (Mouse Look)
/// </summary>
public class FirstPersonCamera : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    [Tooltip("Horizontal sensitivity")]
    public float mouseSensitivityX = 200f;

    [Tooltip("Vertical sensitivity")]
    public float mouseSensitivityY = 200f;

    [Header("Look Limits")]
    [Tooltip("Maximum look up angle (degrees)")]
    public float maxLookUpAngle = 80f;

    [Tooltip("Maximum look down angle (degrees)")]
    public float maxLookDownAngle = 80f;

    [Header("Smoothing Settings")]
    [Tooltip("Enable smoothing")]
    public bool enableSmoothing = true;

    [Tooltip("Smoothing time")]
    public float smoothTime = 0.1f;

    [Header("References")]
    [Tooltip("Camera transform (usually a child object)")]
    public Transform cameraTransform;

    [Tooltip("Player body transform")]
    public Transform playerBody;

    // Private variables
    private float xRotation = 0f; // Vertical camera rotation (pitch)
    private float yRotation = 0f; // Horizontal body rotation (yaw)

    private float currentXRotation = 0f;
    private float currentYRotation = 0f;

    private float xRotationVelocity = 0f;
    private float yRotationVelocity = 0f;

    void Start()
    {
        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Auto-assign references
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (playerBody == null)
        {
            playerBody = transform;
        }

        // Initialize rotation
        xRotation = cameraTransform.localEulerAngles.x;
        yRotation = playerBody.eulerAngles.y;
    }

    void Update()
    {
        // ESC key toggles cursor lock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }

        // Only control camera when cursor is locked
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            LookAround();
        }
    }

    /// <summary>
    /// Mouse look control
    /// </summary>
    void LookAround()
    {
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        // Calculate rotation
        xRotation -= mouseY; // Pitch (up/down)
        yRotation += mouseX; // Yaw (left/right)

        // Clamp pitch angle
        xRotation = Mathf.Clamp(xRotation, -maxLookUpAngle, maxLookDownAngle);

        // Apply rotation
        if (enableSmoothing)
        {
            // Smooth rotation
            currentXRotation = Mathf.SmoothDampAngle(
                currentXRotation,
                xRotation,
                ref xRotationVelocity,
                smoothTime
            );

            currentYRotation = Mathf.SmoothDampAngle(
                currentYRotation,
                yRotation,
                ref yRotationVelocity,
                smoothTime
            );

            cameraTransform.localRotation =
                Quaternion.Euler(currentXRotation, 0f, 0f);

            playerBody.rotation =
                Quaternion.Euler(0f, currentYRotation, 0f);
        }
        else
        {
            // Direct rotation
            cameraTransform.localRotation =
                Quaternion.Euler(xRotation, 0f, 0f);

            playerBody.rotation =
                Quaternion.Euler(0f, yRotation, 0f);
        }
    }

    /// <summary>
    /// Toggle cursor lock state
    /// </summary>
    void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Get current look direction (for other systems)
    /// </summary>
    public Vector3 GetLookDirection()
    {
        return cameraTransform.forward;
    }

    /// <summary>
    /// Set mouse sensitivity
    /// </summary>
    public void SetSensitivity(float sensitivity)
    {
        mouseSensitivityX = sensitivity;
        mouseSensitivityY = sensitivity;
    }
}
