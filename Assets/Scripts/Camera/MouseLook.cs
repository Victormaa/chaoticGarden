using UnityEngine;

/// <summary>
/// 第一人称鼠标视角控制
/// </summary>
public class MouseLook : MonoBehaviour
{
    [Header("视角设置")]
    [Tooltip("鼠标灵敏度")]
    public float mouseSensitivity = 100f;

    [Tooltip("Y轴视角限制（向上）")]
    public float topClamp = 90f;

    [Tooltip("Y轴视角限制（向下）")]
    public float bottomClamp = -90f;

    [Header("引用")]
    [Tooltip("玩家身体（用于水平旋转）")]
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        // 自动查找父对象（如果没有手动分配）
        if (playerBody == null)
        {
            playerBody = transform.parent;
            if (playerBody != null)
            {
                Debug.Log("自动找到 Player Body: " + playerBody.name);
            }
            else
            {
                Debug.LogError("未找到 Player Body！请确保 Camera 是 Player 的子对象");
            }
        }

        // 锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 安全检查
        if (playerBody == null)
        {
            Debug.LogWarning("Player Body 未分配！");
            return;
        }

        // 获取鼠标输入
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Debug 输出（测试用）
        if (Input.GetMouseButton(0)) // 按住鼠标左键时显示
        {
            Debug.Log($"鼠标X: {mouseX}, 鼠标Y: {mouseY}");
        }

        // 垂直旋转（相机上下看）
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, bottomClamp, topClamp);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 水平旋转（玩家身体左右转）
        playerBody.Rotate(Vector3.up * mouseX);

        // ESC解锁鼠标
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // 点击屏幕重新锁定
        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // 显示当前旋转值（Debug用）
    void OnGUI()
    {
        if (playerBody != null)
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"Player Y旋转: {playerBody.eulerAngles.y:F1}°");
            GUI.Label(new Rect(10, 30, 300, 20), $"Camera X旋转: {xRotation:F1}°");
        }
    }
}
