using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private float groundRadius;
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Check")]
    [SerializeField] private float wallRadius;
    [SerializeField] private Transform wallCheckPos;
    [SerializeField] private LayerMask wallLayer;

    [field: Header("Movement Settings")]
    [field: SerializeField] public float walkSpeed { get; private set; } = 7f;
    [field: SerializeField] public float runSpeed { get; private set; } = 12f;
    [field: SerializeField] public float jumpForce { get; private set; } = 25f;
    [field: SerializeField] public float wallSlideSpeed { get; private set; } = 0.15f;
    [field: SerializeField] public float rollGroundSpeed { get; private set; } = 15f;
    [field: SerializeField] public float rollGroundTime { get; private set; } = 0.2f;
    [field: SerializeField] public float rollAirSpeed { get; private set; } = 15f;
    [field: SerializeField] public float rollAirTime { get; private set; } = 0.2f;
    [field: SerializeField] public float rollCooldown { get; private set; } = 0.2f;
    [field: SerializeField] public float jumpBufferTime { get; private set; } = 0.15f;

    [field: Header("Wall Jump Settings")]
    [field: SerializeField] public float wallJumpTime { get; private set; } = 0.2f;
    [field: SerializeField] public float wallJumpDuration { get; private set; } = 0.4f;
    [field: SerializeField] public Vector2 wallJumpPower { get; private set; } = new Vector2(8f, 16f);

    [HideInInspector] public bool canFlip = true;
    [HideInInspector] public bool isWallJumping;
    [HideInInspector] public float wallJumpDir;
    [HideInInspector] public float wallJumpCounter;
    [HideInInspector] public bool isGrounded { get; private set; }
    [HideInInspector] public bool isOnWall { get; private set; }
    [HideInInspector] public Rigidbody2D rb { get; private set; }
    [HideInInspector] public Animator animator { get; private set; }
    [HideInInspector] public Vector2 moveDir;
    [HideInInspector] public bool isSprinting;
    [HideInInspector] public bool isWallSliding;
    [HideInInspector] public bool _facingRight = true;
    [HideInInspector] public float currentRollCooldownTimer;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (canFlip)
        {
            if (moveDir.x > 0 && !_facingRight) Flip();
            else if (moveDir.x < 0 && _facingRight) Flip();
        }

        GroundWallCheck();

        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        if (!isWallSliding)
        {
            wallJumpCounter = Mathf.Max(0f, wallJumpCounter - Time.deltaTime);
        }

        if (currentRollCooldownTimer > 0) currentRollCooldownTimer -= Time.deltaTime;
    }

    void GroundWallCheck()
    {
        Collider2D groundHits = Physics2D.OverlapCircle(groundCheckPos.position, groundRadius, groundLayer);
        Collider2D wallHits = Physics2D.OverlapCircle(wallCheckPos.position, wallRadius, wallLayer);
        isGrounded = groundHits != null;
        isOnWall = wallHits != null;
    }

    void Flip()
    {
        _facingRight = !_facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(groundCheckPos.position, groundRadius);

        Gizmos.color = isOnWall ? Color.green : Color.red;
        Gizmos.DrawWireSphere(wallCheckPos.position, wallRadius);
    }
}
