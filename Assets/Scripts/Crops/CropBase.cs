using UnityEngine;

public enum CropState
{
    Growing,
    Mature,
    Harvesting,
    Depleted
}

/// <summary>
/// 3D作物基类
/// </summary>
public abstract class CropBase : MonoBehaviour
{
    [Header("作物基础属性")]
    public string cropName = "未命名作物";
    public int baseValue = 10;

    [Header("生长设置")]
    public float growthTime = 5f;
    public CropState currentState = CropState.Growing;

    [Header("视觉反馈")]
    public Color growingColor = Color.gray;
    public Color matureColor = Color.green;
    public Color harvestingColor = Color.yellow;

    [Header("模型引用")]
    public GameObject modelRoot; // 3D模型根节点（必须设置！）

    protected Renderer[] renderers;
    protected float growthTimer = 0f;

    protected virtual void Awake()
    {
        // 如果没有手动指定，尝试查找名为 "Model" 的子对象
        if (modelRoot == null)
        {
            Transform modelTransform = transform.Find("Model");
            if (modelTransform != null)
            {
                modelRoot = modelTransform.gameObject;
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: 未找到 'Model' 子对象，将使用自身作为模型根节点");
                modelRoot = gameObject;
            }
        }

        // 获取所有渲染器
        if (modelRoot != null)
        {
            renderers = modelRoot.GetComponentsInChildren<Renderer>();
        }
    }

    protected virtual void Start()
    {
        currentState = CropState.Growing;
        UpdateVisuals();
    }

    protected virtual void Update()
    {
        if (currentState == CropState.Growing)
        {
            GrowthUpdate();
        }
    }

    /// <summary>
    /// 生长更新
    /// </summary>
    protected virtual void GrowthUpdate()
    {
        growthTimer += Time.deltaTime;

        if (growthTimer >= growthTime)
        {
            currentState = CropState.Mature;
            UpdateVisuals();
            OnMature();
        }
        else
        {
            // 生长动画：缩放
            if (modelRoot != null)
            {
                float t = growthTimer / growthTime;
                modelRoot.transform.localScale = Vector3.Lerp(Vector3.one * 0.3f, Vector3.one, t);
            }
        }
    }

    /// <summary>
    /// 成熟时调用
    /// </summary>
    protected virtual void OnMature()
    {
        Debug.Log($"{cropName} 已成熟！");

        // 成熟特效：轻微弹跳
        if (modelRoot != null)
        {
            StartCoroutine(BounceEffect());
        }
    }

    /// <summary>
    /// 弹跳效果
    /// </summary>
    protected System.Collections.IEnumerator BounceEffect()
    {
        Vector3 originalScale = modelRoot.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;

        float duration = 0.2f;
        float elapsed = 0f;

        // 放大
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            modelRoot.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        elapsed = 0f;

        // 缩小
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            modelRoot.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        modelRoot.transform.localScale = originalScale;
    }

    /// <summary>
    /// 收获作物
    /// </summary>
    public virtual void Harvest()
    {
        if (currentState != CropState.Mature)
        {
            Debug.LogWarning($"{cropName} 尚未成熟！");
            return;
        }

        currentState = CropState.Harvesting;
        UpdateVisuals();
        OnHarvest();
    }

    /// <summary>
    /// 收获时调用（子类必须实现）
    /// </summary>
    protected abstract void OnHarvest();

    /// <summary>
    /// 更新视觉效果
    /// </summary>
    protected virtual void UpdateVisuals()
    {
        if (renderers == null || renderers.Length == 0) return;

        Color targetColor = currentState switch
        {
            CropState.Growing => growingColor,
            CropState.Mature => matureColor,
            CropState.Harvesting => harvestingColor,
            _ => Color.white
        };

        foreach (var renderer in renderers)
        {
            if (renderer != null && renderer.material != null)
            {
                renderer.material.color = targetColor;
            }
        }
    }

    /// <summary>
    /// Gizmos 调试显示
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = currentState == CropState.Mature ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
