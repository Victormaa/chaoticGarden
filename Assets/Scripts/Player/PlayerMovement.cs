using UnityEngine;

/// <summary>
/// 第一人称玩家移动控制器
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    [Tooltip("行走速度")]
    public float walkSpeed = 6f;

    [Tooltip("奔跑速度")]
    public float sprintSpeed = 10f;

    [Tooltip("下蹲速度")]
    public float crouchSpeed = 3f;

    [Tooltip("加速度")]
    public float acceleration = 10f;

    [Header("跳跃设置")]
    [Tooltip("启用跳跃")]
    public bool enableJump = true;

    [Tooltip("跳跃高度")]
    public float jumpHeight = 2f;

    [Tooltip("重力")]
    public float gravity = -20f;

    [Header("下蹲设置")]
    [Tooltip("启用下蹲")]
    public bool enableCrouch = true;

    [Tooltip("下蹲高度")]
    public float crouchHeight = 1f;

    [Tooltip("站立高度")]
    public float standHeight = 2f;

    [Header("输入键位")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("引用")]
    public Transform cameraTransform;

    // 私有变量
    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 moveDirection;
    private float currentSpeed;
    private bool isGrounded;
    private bool isCrouching;
    private bool isSprinting;

    void Awake()
    {
        // 获取或添加 CharacterController
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            Debug.Log("已自动添加 CharacterController");
        }

        // 查找相机
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Start()
    {
        // 配置 CharacterController
        controller.height = standHeight;
        controller.center = new Vector3(0, standHeight / 2, 0);
        controller.radius = 0.5f;
    }

    void Update()
    {
        CheckGrounded();
        HandleMovement();
        HandleCrouch();
        HandleJump();
        ApplyGravity();
    }

    /// <summary>
    /// 检测是否在地面
    /// </summary>
    void CheckGrounded()
    {
        // 使用 CharacterController 的 isGrounded
        isGrounded = controller.isGrounded;

        // 额外的射线检测（更可靠）
        float rayDistance = (controller.height / 2) + 0.1f;
        Ray ray = new Ray(transform.position, Vector3.down);
        isGrounded = Physics.Raycast(ray, rayDistance);

        // 如果在地面且向下速度，重置速度
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 小的负值保持贴地
        }
    }

    /// <summary>
    /// 处理移动
    /// </summary>
    void HandleMovement()
    {
        // 获取输入
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D
        float vertical = Input.GetAxisRaw("Vertical");     // W/S

        // 计算移动方向（相对于相机）
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // 忽略Y轴
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // 合成移动方向
        moveDirection = (forward * vertical + right * horizontal).normalized;

        // 确定速度
        if (Input.GetKey(sprintKey) && !isCrouching)
        {
            currentSpeed = sprintSpeed;
            isSprinting = true;
        }
        else if (isCrouching)
        {
            currentSpeed = crouchSpeed;
            isSprinting = false;
        }
        else
        {
            currentSpeed = walkSpeed;
            isSprinting = false;
        }

        // 移动
        Vector3 move = moveDirection * currentSpeed;
        controller.Move(move * Time.deltaTime);
    }

    /// <summary>
    /// 处理下蹲
    /// </summary>
    void HandleCrouch()
    {
        if (!enableCrouch) return;

        if (Input.GetKeyDown(crouchKey))
        {
            isCrouching = true;
            controller.height = crouchHeight;
            controller.center = new Vector3(0, crouchHeight / 2, 0);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            // 检查头顶是否有障碍物
            if (!Physics.Raycast(transform.position, Vector3.up, standHeight))
            {
                isCrouching = false;
                controller.height = standHeight;
                controller.center = new Vector3(0, standHeight / 2, 0);
            }
        }
    }

    /// <summary>
    /// 处理跳跃
    /// </summary>
    void HandleJump()
    {
        if (!enableJump) return;

        if (Input.GetKeyDown(jumpKey) && isGrounded && !isCrouching)
        {
            // 跳跃公式：v = sqrt(2 * height * gravity)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    /// <summary>
    /// 应用重力
    /// </summary>
    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// 获取移动速度（用于音效/动画）
    /// </summary>
    public float GetMoveSpeed()
    {
        Vector3 horizontalVelocity = controller.velocity;
        horizontalVelocity.y = 0;
        return horizontalVelocity.magnitude;
    }

    /// <summary>
    /// 是否在移动
    /// </summary>
    public bool IsMoving()
    {
        return moveDirection.magnitude > 0.1f;
    }

    /// <summary>
    /// 是否在地面
    /// </summary>
    public bool IsGrounded()
    {
        return isGrounded;
    }

    /// <summary>
    /// 是否在奔跑
    /// </summary>
    public bool IsSprinting()
    {
        return isSprinting;
    }

    /// <summary>
    /// Gizmos 显示
    /// </summary>
    void OnDrawGizmos()
    {
        if (Application.isPlaying && controller != null)
        {
            // 显示移动方向
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, moveDirection * 2f);

            // 显示地面检测
            Gizmos.color = isGrounded ? Color.green : Color.red;
            float rayDistance = (controller.height / 2) + 0.1f;
            Gizmos.DrawRay(transform.position, Vector3.down * rayDistance);
        }
    }
}
