using UnityEngine;
using System.Collections;

/// <summary>
/// 3D窜稀甜菜
/// </summary>
public class BeetBurst : CropBase
{
    [Header("窜稀甜菜设置")]
    [Tooltip("爆发的甜菜数量")]
    public int burstAmount = 50;

    [Tooltip("爆发持续时间")]
    public float burstDuration = 2f;

    [Tooltip("价值衰减时间")]
    public float decayTime = 20f;

    [Header("爆发物理")]
    [Tooltip("最小爆发力")]
    public float minForce = 5f;

    [Tooltip("最大爆发力")]
    public float maxForce = 15f;

    [Tooltip("向上的力")]
    public float upwardForce = 8f;

    [Tooltip("爆发半径")]
    public float burstRadius = 8f;

    [Header("预制体")]
    [Tooltip("甜菜可收集物预制体（可选）")]
    public GameObject beetCollectiblePrefab;

    [Header("特效")]
    public GameObject explosionVFXPrefab;
    public ParticleSystem dirtParticles;

    protected override void Start()
    {
        base.Start();
        cropName = "窜稀甜菜";

        // 如果没有预制体，运行时创建
        if (beetCollectiblePrefab == null)
        {
            beetCollectiblePrefab = CreateBeetPrefab();
        }
    }

    protected override void OnHarvest()
    {
        StartCoroutine(BeetExplosionSequence());
    }

    /// <summary>
    /// 甜菜爆发序列
    /// </summary>
    IEnumerator BeetExplosionSequence()
    {
        // 1. 拔出动画
        yield return StartCoroutine(PullOutAnimation());

        // 2. 泥土粒子特效
        if (dirtParticles != null)
        {
            dirtParticles.Play();
        }

        // 3. 镜头震动
        if (CameraShaker.Instance != null)
        {
            CameraShaker.Instance.ShakeHeavy(2f);
        }

        // 4. 爆发甜菜
        float spawnInterval = burstDuration / burstAmount;

        for (int i = 0; i < burstAmount; i++)
        {
            SpawnBeet();

            // 每10个加一次小震动
            if (i % 10 == 0 && CameraShaker.Instance != null)
            {
                CameraShaker.Instance.ShakeLight(0.1f);
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        // 5. 销毁作物
        Destroy(gameObject, 1f);
    }

    /// <summary>
    /// 拔出动画
    /// </summary>
    IEnumerator PullOutAnimation()
    {
        if (modelRoot == null)
        {
            Debug.LogWarning("BeetBurst: modelRoot 未设置！");
            yield break;
        }

        Vector3 originalPos = modelRoot.transform.localPosition;
        float elapsed = 0;
        float duration = 0.5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 上下抖动 + 上升
            float shake = Mathf.Sin(t * Mathf.PI * 8) * 0.1f;
            float lift = t * 0.5f;

            modelRoot.transform.localPosition = originalPos + new Vector3(0, lift + shake, 0);

            // 旋转
            modelRoot.transform.Rotate(Vector3.up, 720 * Time.deltaTime);

            yield return null;
        }
    }

    /// <summary>
    /// 生成单个甜菜
    /// </summary>
    void SpawnBeet()
    {
        if (beetCollectiblePrefab == null)
        {
            Debug.LogError("BeetBurst: 甜菜预制体未设置！");
            return;
        }

        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        GameObject beet = Instantiate(beetCollectiblePrefab, spawnPos, Random.rotation);

        // 随机方向（球形）
        Vector3 randomDir = Random.onUnitSphere;
        randomDir.y = Mathf.Abs(randomDir.y); // 确保向上
        randomDir = randomDir.normalized;

        // 应用力
        Rigidbody rb = beet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            float force = Random.Range(minForce, maxForce);
            rb.AddForce(randomDir * force + Vector3.up * upwardForce, ForceMode.Impulse);

            // 随机旋转
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
        }

        // 添加价值衰减
        ValueDecay decay = beet.GetComponent<ValueDecay>();
        if (decay == null)
        {
            decay = beet.AddComponent<ValueDecay>();
        }
        decay.Setup(baseValue, decayTime);
    }

    /// <summary>
    /// 创建甜菜预制体（临时方案）
    /// </summary>
    GameObject CreateBeetPrefab()
    {
        GameObject prefab = new GameObject("BeetCollectible");

        // 添加视觉（球体）
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visual.transform.SetParent(prefab.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = Vector3.one * 0.3f;

        // 移除球体自带碰撞器
        Collider sphereCollider = visual.GetComponent<Collider>();
        if (sphereCollider != null)
        {
            Destroy(sphereCollider);
        }

        // 设置材质
        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.8f, 0.2f, 0.3f); // 红色

        Renderer renderer = visual.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = mat;
        }

        // 添加物理
        Rigidbody rb = prefab.AddComponent<Rigidbody>();
        rb.mass = 0.5f;
        rb.drag = 1f;
        rb.angularDrag = 0.5f;

        // 添加触发器碰撞器
        SphereCollider col = prefab.AddComponent<SphereCollider>();
        col.radius = 0.15f;
        col.isTrigger = true;

        // 添加可收集组件
        prefab.AddComponent<Collectible>();

        // 设置标签
        prefab.tag = "Collectible";

        return prefab;
    }
}
