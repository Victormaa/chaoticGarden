using UnityEngine;
using Cinemachine;

/// <summary>
/// 第三人称俯视角相机控制器
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("镜头引用")]
    public CinemachineVirtualCamera virtualCamera;

    [Header("俯视角设置")]
    [Tooltip("相机高度")]
    public float cameraHeight = 15f;

    [Tooltip("相机后方偏移")]
    public float cameraBackOffset = -8f;

    [Tooltip("俯视角度（度）")]
    [Range(30f, 90f)]
    public float viewAngle = 60f;

    [Header("缩放设置")]
    public bool enableZoom = true;
    public float minZoom = 10f;
    public float maxZoom = 25f;
    public float zoomSpeed = 2f;
    private float currentZoom;

    [Header("旋转设置（可选）")]
    public bool enableRotation = false;
    public float rotationSpeed = 100f;
    private float currentRotation = 0f;

    private CinemachineTransposer transposer;

    void Start()
    {
        // 如果没有手动指定，尝试获取组件
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        // 如果还是没有，查找场景中的虚拟相机
        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }

        if (virtualCamera != null)
        {
            transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        }
        else
        {
            Debug.LogError("CameraController: 未找到 CinemachineVirtualCamera！");
            enabled = false;
            return;
        }

        currentZoom = cameraHeight;
        UpdateCameraPosition();
    }

    void Update()
    {
        HandleZoom();
        HandleRotation();
    }

    /// <summary>
    /// 处理滚轮缩放
    /// </summary>
    void HandleZoom()
    {
        if (!enableZoom) return;

        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scrollInput) > 0.01f)
        {
            currentZoom -= scrollInput * zoomSpeed;
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

            UpdateCameraPosition();
        }
    }

    /// <summary>
    /// 处理Q/E键旋转（可选功能）
    /// </summary>
    void HandleRotation()
    {
        if (!enableRotation) return;

        if (Input.GetKey(KeyCode.Q))
        {
            currentRotation += rotationSpeed * Time.deltaTime;
            UpdateCameraPosition();
        }
        else if (Input.GetKey(KeyCode.E))
        {
            currentRotation -= rotationSpeed * Time.deltaTime;
            UpdateCameraPosition();
        }
    }

    /// <summary>
    /// 更新相机位置和角度
    /// </summary>
    void UpdateCameraPosition()
    {
        if (transposer == null) return;

        // 计算偏移量
        float angleRad = viewAngle * Mathf.Deg2Rad;
        float yOffset = currentZoom;
        float zOffset = -currentZoom / Mathf.Tan(angleRad);

        // 应用旋转
        Vector3 offset = Quaternion.Euler(0, currentRotation, 0) * new Vector3(0, yOffset, zOffset);

        transposer.m_FollowOffset = offset;
    }

    /// <summary>
    /// 重置镜头
    /// </summary>
    public void ResetCamera()
    {
        currentZoom = cameraHeight;
        currentRotation = 0f;
        UpdateCameraPosition();
    }
}
