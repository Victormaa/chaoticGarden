using UnityEngine;

public class Collectible : MonoBehaviour
{
    [Header("物品数据")]
    public string itemID;
    public int amount = 1;

    [Header("拾取设置")]
    public float pickupDistance = 2f;
    public KeyCode pickupKey = KeyCode.F;
    public bool autoPickup = false;

    [Header("物理设置")]
    public float initialDrag = 1f;           // 初始阻力
    public float groundDragMultiplier = 3f;  // 落地后阻力倍数

    private Transform player;
    private Rigidbody rb;
    private bool isCollected = false;
    private bool canPickup = false;
    private float originalDrag;
    private float originalAngularDrag;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            originalDrag = rb.drag;
            originalAngularDrag = rb.angularDrag;
        }

        Invoke(nameof(EnablePickup), 0.5f);
    }

    void EnablePickup()
    {
        canPickup = true;
    }

    void Update()
    {
        if (isCollected || !canPickup || player == null) return;

        float distance = Vector3.Distance(player.position, transform.position);

        // 拾取检测
        if (distance <= pickupDistance)
        {
            if (Input.GetKeyDown(pickupKey))
            {
                Collect();
            }
        }
    }

    //  碰到地面/物体时增加阻力（保留物理但快速停止）
    void OnCollisionEnter(Collision collision)
    {
        if (rb != null)
        {
            // 渐进式增加阻力，而不是立即停止
            rb.drag = originalDrag * groundDragMultiplier;
            rb.angularDrag = originalAngularDrag * groundDragMultiplier;
        }
    }

    void Collect()
    {
        if (isCollected) return;
        isCollected = true;

        if (InventoryManager.Instance != null)
        {
            bool success = InventoryManager.Instance.AddItem(itemID, amount);
            if (success)
            {
                Debug.Log($"拾取 {itemID} x{amount}");
            }
            else
            {
                Debug.LogWarning("背包已满！");
                isCollected = false;
                return;
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, pickupDistance);
    }
}