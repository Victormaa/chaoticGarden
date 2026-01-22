using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 6.5f;      // Ultra-stable configuration
    public float gravity = -20f;         // Ultra-stable configuration
    public float jumpCooldown = 0.3f;    // Ultra-stable configuration

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 80f;

    [Header("Ground Detection Settings")]
    public Transform groundCheck;
    public float groundDistance = 0.25f; // Ultra-stable configuration
    public LayerMask groundMask;

    [Header("Animation Settings")]
    public Animator animator;
    public float animationSmoothTime = 0.1f;

    [Header("References")]
    public Transform cameraTransform;

    [Header("Debug Options")]
    public bool showDebugInfo = false;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private bool isGrounded;
    private bool wasGroundedLastFrame;
    private bool isSprinting;
    private float lastJumpTime = -999f;

    private float currentMoveX;
    private float currentMoveZ;
    private float velocityMoveX;
    private float velocityMoveZ;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError("Missing CharacterController component!");
            enabled = false;
            return;
        }

        // Optimize CharacterController settings
        controller.skinWidth = 0.08f;
        controller.minMoveDistance = 0.001f;

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (groundCheck == null)
        {
            GameObject checkObj = new GameObject("GroundCheck");
            checkObj.transform.SetParent(transform);
            float checkY = -controller.height / 2f + controller.center.y - 0.05f;
            checkObj.transform.localPosition = new Vector3(0, checkY, 0);
            groundCheck = checkObj.transform;
        }

        if (groundMask == 0)
        {
            Debug.LogWarning("Ground Mask not set!");
        }
    }

    void Update()
    {
        CheckGround();
        HandleMovement();
        HandleMouseLook();
        UpdateAnimation();

        wasGroundedLastFrame = isGrounded;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    void HandleMovement()
    {
        // Landing handling
        if (isGrounded)
        {
            if (!wasGroundedLastFrame)
            {
                // Just landed, completely stop vertical velocity
                velocity.y = 0f;
            }
            else if (velocity.y < 0)
            {
                // Continuously on ground, slight downward pressure
                velocity.y = -2f;
            }
        }

        // Movement input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Horizontal movement
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump detection
        bool jumpPressed = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space);
        bool canJump = isGrounded && (Time.time >= lastJumpTime + jumpCooldown);

        if (jumpPressed && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            lastJumpTime = Time.time;

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }

        // Apply gravity
        if (!isGrounded || velocity.y > 0)
        {
            velocity.y += gravity * Time.deltaTime;
            velocity.y = Mathf.Max(velocity.y, -40f);
        }

        // Apply vertical movement
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        float targetMoveX = inputX;
        float targetMoveZ = 0f;

        if (Mathf.Abs(inputZ) > 0.1f || Mathf.Abs(inputX) > 0.1f)
        {
            if (isSprinting)
            {
                targetMoveZ = 2f;
            }
            else
            {
                targetMoveZ = 1f;
            }

            if (Mathf.Abs(inputZ) < 0.1f)
            {
                targetMoveZ *= 0.5f;
            }
        }
        else
        {
            targetMoveZ = 0f;
            targetMoveX = 0f;
        }

        currentMoveX = Mathf.SmoothDamp(currentMoveX, targetMoveX, ref velocityMoveX, animationSmoothTime);
        currentMoveZ = Mathf.SmoothDamp(currentMoveZ, targetMoveZ, ref velocityMoveZ, animationSmoothTime);

        animator.SetFloat("MoveX", currentMoveX);
        animator.SetFloat("MoveZ", currentMoveZ);
        animator.SetBool("IsSprinting", isSprinting);
        animator.SetBool("IsGrounded", isGrounded);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}   