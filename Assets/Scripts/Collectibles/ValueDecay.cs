using UnityEngine;
using System.Collections;

/// <summary>
/// 可收集物价值衰减（3D版本）
/// </summary>
public class ValueDecay : MonoBehaviour
{
    [Header("价值设置")]
    [Tooltip("初始价值")]
    public int startValue = 10;

    [Tooltip("最小价值")]
    public int minValue = 1;

    [Tooltip("当前价值")]
    public int currentValue;

    [Header("衰减设置")]
    [Tooltip("衰减总时间")]
    public float decayDuration = 20f;

    private float decayTimer = 0f;

    [Header("视觉反馈")]
    public Gradient colorGradient;

    [Tooltip("启用浮动文字")]
    public bool enableFloatingText = true;

    private Renderer[] renderers;
    private bool isDecaying = false;

    // 浮动文字
    private TextMesh floatingText;
    private GameObject floatingTextObj;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        // 创建默认渐变
        if (colorGradient == null)
        {
            CreateDefaultGradient();
        }
    }

    void Start()
    {
        currentValue = startValue;

        if (enableFloatingText)
        {
            CreateFloatingText();
        }
    }

    /// <summary>
    /// 创建默认颜色渐变
    /// </summary>
    void CreateDefaultGradient()
    {
        colorGradient = new Gradient();

        GradientColorKey[] colorKeys = new GradientColorKey[3];
        colorKeys[0] = new GradientColorKey(new Color(1f, 0.84f, 0f), 0f); // 金色
        colorKeys[1] = new GradientColorKey(Color.white, 0.5f);
        colorKeys[2] = new GradientColorKey(new Color(0.5f, 0.5f, 0.5f), 1f); // 灰色

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1f, 0f);
        alphaKeys[1] = new GradientAlphaKey(0.7f, 1f);

        colorGradient.SetKeys(colorKeys, alphaKeys);
    }

    /// <summary>
    /// 设置衰减参数并开始衰减
    /// </summary>
    public void Setup(int value, float duration)
    {
        startValue = value;
        currentValue = value;
        decayDuration = duration;
        isDecaying = true;

        // 更新浮动文字
        if (floatingText != null)
        {
            floatingText.text = $"${currentValue}";
        }

        StartCoroutine(DecayRoutine());
    }

    /// <summary>
    /// 衰减协程
    /// </summary>
    IEnumerator DecayRoutine()
    {
        while (decayTimer < decayDuration)
        {
            decayTimer += Time.deltaTime;
            float t = decayTimer / decayDuration;

            // 线性衰减
            currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, minValue, t));

            // 更新颜色
            UpdateColor(t);

            // 更新浮动文字
            if (floatingText != null)
            {
                floatingText.text = $"${currentValue}";
                floatingText.color = colorGradient.Evaluate(t);
            }

            yield return null;
        }

        currentValue = minValue;
        isDecaying = false;
    }

    /// <summary>
    /// 更新颜色
    /// </summary>
    void UpdateColor(float t)
    {
        Color currentColor = colorGradient.Evaluate(t);

        foreach (var renderer in renderers)
        {
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = currentColor;
            }
        }
    }

    /// <summary>
    /// 创建3D浮动文字
    /// </summary>
    void CreateFloatingText()
    {
        floatingTextObj = new GameObject("ValueText");
        floatingTextObj.transform.SetParent(transform);
        floatingTextObj.transform.localPosition = Vector3.up * 0.5f;

        floatingText = floatingTextObj.AddComponent<TextMesh>();
        floatingText.text = $"${startValue}";
        floatingText.fontSize = 50;
        floatingText.anchor = TextAnchor.MiddleCenter;
        floatingText.alignment = TextAlignment.Center;
        floatingText.color = Color.yellow;

        // 让文字始终面向相机
        Billboard billboard = floatingTextObj.GetComponent<Billboard>();
        if (billboard == null)
        {
            floatingTextObj.AddComponent<Billboard>();
        }
    }

    /// <summary>
    /// 获取当前价值
    /// </summary>
    public int GetValue()
    {
        return currentValue;
    }

    /// <summary>
    /// 是否正在衰减
    /// </summary>
    public bool IsDecaying()
    {
        return isDecaying;
    }

    void OnDestroy()
    {
        if (floatingTextObj != null)
        {
            Destroy(floatingTextObj);
        }
    }
}
