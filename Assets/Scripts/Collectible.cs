using UnityEngine;

/// <summary>
/// 3D可收集物
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class Collectible : MonoBehaviour
{
    [Header("收集设置")]
    [Tooltip("自动收集（触碰即收集）")]
    public bool autoCollect = true;

    [Tooltip("生成后多久可以被收集")]
    public float collectDelay = 0.5f;

    [Header("磁吸效果")]
    [Tooltip("启用磁吸效果")]
    public bool enableMagnet = true;

    [Tooltip("磁吸范围")]
    public float magnetRange = 3f;

    [Tooltip("磁吸速度")]
    public float magnetSpeed = 10f;

    [Header("音效")]
    public AudioClip collectSound;

    [Header("特效")]
    public GameObject collectVFXPrefab;

    // 私有变量
    private bool canBeCollected = false;
    private ValueDecay valueDecay;
    private Rigidbody rb;
    private Transform playerTransform;
    private bool isBeingPulled = false;

    void Start()
    {
        // 获取组件
        valueDecay = GetComponent<ValueDecay>();
        rb = GetComponent<Rigidbody>();

        // 确保碰撞器是触发器
        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        // 延迟启用收集
        Invoke(nameof(EnableCollection), collectDelay);
    }

    void EnableCollection()
    {
        canBeCollected = true;
    }

    void Update()
    {
        if (!canBeCollected) return;

        // 磁吸检测
        if (enableMagnet && !isBeingPulled)
        {
            CheckForPlayer();
        }

        // 移动向玩家
        if (isBeingPulled && playerTransform != null)
        {
            MoveTowardsPlayer();
        }
    }

    /// <summary>
    /// 检测玩家是否在磁吸范围内
    /// </summary>
    void CheckForPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, magnetRange);

        foreach (var col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                playerTransform = col.transform;
                isBeingPulled = true;

                // 禁用物理，改用Transform移动
                if (rb != null)
                {
                    rb.isKinematic = true;
                }

                break;
            }
        }
    }

    /// <summary>
    /// 移动向玩家
    /// </summary>
    void MoveTowardsPlayer()
    {
        if (playerTransform == null) return;

        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * magnetSpeed * Time.deltaTime;

        // 旋转效果
        transform.Rotate(Vector3.up, 360f * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!canBeCollected) return;

        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }

    /// <summary>
    /// 执行收集
    /// </summary>
    void Collect(GameObject player)
    {
        // 获取价值
        int value = 1; // 默认值
        if (valueDecay != null)
        {
            value = valueDecay.GetValue();
        }

        // 查找玩家收集器
        PlayerCollector collector = player.GetComponent<PlayerCollector>();
        if (collector != null)
        {
            collector.CollectItem(value, transform.position);
        }
        else
        {
            Debug.LogWarning("Player 上未找到 PlayerCollector 组件！");
        }

        // 播放音效
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }

        // 生成特效
        SpawnCollectVFX();

        // 销毁自己
        Destroy(gameObject);
    }

    /// <summary>
    /// 生成收集特效
    /// </summary>
    void SpawnCollectVFX()
    {
        if (collectVFXPrefab != null)
        {
            GameObject vfx = Instantiate(collectVFXPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, 2f);
        }
        else
        {
            // 创建临时粒子特效
            CreateDefaultCollectVFX();
        }
    }

    /// <summary>
    /// 创建默认收集特效
    /// </summary>
    void CreateDefaultCollectVFX()
    {
        GameObject vfx = new GameObject("CollectVFX");
        vfx.transform.position = transform.position;

        ParticleSystem ps = vfx.AddComponent<ParticleSystem>();

        // Main 模块
        var main = ps.main;
        main.startLifetime = 0.5f;
        main.startSpeed = 3f;
        main.startSize = 0.2f;
        main.startColor = new Color(1f, 0.84f, 0f); // 金色
        main.gravityModifier = -0.5f; // 向上飘
        main.maxParticles = 20;

        // Emission 模块
        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0, 15)
        });

        // Shape 模块
        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.3f;

        // 播放并销毁
        ps.Play();
        Destroy(vfx, 2f);
    }

    /// <summary>
    /// Gizmos显示磁吸范围
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (enableMagnet)
        {
            Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, magnetRange);
        }
    }
}
