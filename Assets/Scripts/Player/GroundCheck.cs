using UnityEngine;

/// <summary>
/// Ground Check (ensures the player stays grounded)
/// </summary>
public class GroundCheck : MonoBehaviour
{
    [Header("Check Settings")]//µÿ√ÊºÏ≤‚
    public LayerMask groundLayer;
    public float groundDistance = 0.2f;
    public Transform checkPoint; // Check point (usually near the feet)

    [Header("State")]
    [SerializeField] private bool isGrounded;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (checkPoint == null)
        {
            // Create check point
            GameObject cp = new GameObject("GroundCheckPoint");
            cp.transform.parent = transform;
            cp.transform.localPosition = new Vector3(0, 0.1f, 0);
            checkPoint = cp.transform;
        }
    }

    void FixedUpdate()
    {
        CheckGround();
    }

    void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            checkPoint.position,
            groundDistance,
            groundLayer
        );

        // If not grounded, snap back to ground
        if (!isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(
                transform.position + Vector3.up,
                Vector3.down,
                out hit,
                10f,
                groundLayer
            ))
            {
                transform.position = new Vector3(
                    transform.position.x,
                    hit.point.y,
                    transform.position.z
                );
            }
        }
    }

    void OnDrawGizmos()
    {
        if (checkPoint == null) return;

        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(checkPoint.position, groundDistance);
    }
}
