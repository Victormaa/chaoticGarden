using UnityEngine;
using Cinemachine;
using System.Collections;

/// <summary>
/// 相机震动管理器（单例）
/// </summary>
public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance { get; private set; }

    [Header("震动设置")]
    public CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    [Header("预设震动强度")]
    public float lightShake = 1f;
    public float mediumShake = 3f;
    public float heavyShake = 5f;

    void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 查找虚拟相机
        if (virtualCamera == null)
        {
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
        }

        if (virtualCamera == null)
        {
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }

        if (virtualCamera == null)
        {
            Debug.LogError("CameraShaker: 未找到 CinemachineVirtualCamera！");
            return;
        }

        // 获取或添加噪音组件
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (noise == null)
        {
            noise = virtualCamera.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            Debug.Log("CameraShaker: 已自动添加 Noise 组件");
        }

        // 确保初始状态为0
        if (noise != null)
        {
            noise.m_AmplitudeGain = 0f;
            noise.m_FrequencyGain = 1f;
        }
    }

    /// <summary>
    /// 触发震动
    /// </summary>
    /// <param name="intensity">震动强度</param>
    /// <param name="duration">持续时间</param>
    public void Shake(float intensity, float duration)
    {
        if (noise == null)
        {
            Debug.LogWarning("CameraShaker: Noise 组件未初始化！");
            return;
        }

        StartCoroutine(ShakeRoutine(intensity, duration));
    }

    /// <summary>
    /// 预设震动：轻微
    /// </summary>
    public void ShakeLight(float duration = 0.2f)
    {
        Shake(lightShake, duration);
    }

    /// <summary>
    /// 预设震动：中等
    /// </summary>
    public void ShakeMedium(float duration = 0.5f)
    {
        Shake(mediumShake, duration);
    }

    /// <summary>
    /// 预设震动：剧烈
    /// </summary>
    public void ShakeHeavy(float duration = 1f)
    {
        Shake(heavyShake, duration);
    }

    IEnumerator ShakeRoutine(float intensity, float duration)
    {
        if (noise == null) yield break;

        noise.m_AmplitudeGain = intensity;
        noise.m_FrequencyGain = 1f;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 衰减震动
            float t = 1f - (elapsed / duration);
            noise.m_AmplitudeGain = intensity * t;

            yield return null;
        }

        noise.m_AmplitudeGain = 0f;
    }

    /// <summary>
    /// 立即停止震动
    /// </summary>
    public void StopShake()
    {
        StopAllCoroutines();
        if (noise != null)
        {
            noise.m_AmplitudeGain = 0f;
        }
    }
}
