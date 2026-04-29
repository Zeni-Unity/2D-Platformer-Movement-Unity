using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [field: SerializeField, Tooltip("If enabled, roll is used as a dash with ghost frames.")] public bool useRollAsDash { get; private set; } = false;
    [field: SerializeField] public float ghostFrameStartOpacity { get; private set; } = 0.5f;
    [field: SerializeField] public float ghostFrameDeathTime { get; private set; } = 0.25f;
    [field: SerializeField] public float ghostFrameSpawnTime { get; private set; } = 0.05f;
    [field: SerializeField] public Color ghostFrameColor { get; private set; } = Color.cyan;
    [Header("Ground Check")]
    [SerializeField] private float groundRadius;
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private LayerMask groundLayer;

    [Header("Wall Check")]
    [SerializeField] private float wallRadius;
    [SerializeField] private Transform wallCheckPos;
    [SerializeField] private LayerMask wallLayer;

    [field: Header("Movement Settings")]
    [field: SerializeField, Tooltip("Horizontal movement speed while walking.")] public float walkSpeed { get; private set; } = 7f;
    [field: SerializeField, Tooltip("Horizontal movement speed while sprinting.")] public float runSpeed { get; private set; } = 12f;
    [field: SerializeField, Tooltip("Upward velocity applied instantly when jumping.")] public float jumpForce { get; private set; } = 25f;
    [field: SerializeField, Tooltip("Gravity scale applied while wall sliding (lower = slower slide).")] public float wallSlideSpeed { get; private set; } = 0.15f;
    [field: SerializeField, Tooltip("Impulse force applied when rolling on the ground.")] public float rollGroundSpeed { get; private set; } = 15f;
    [field: SerializeField, Tooltip("Duration of the ground roll in seconds.")] public float rollGroundTime { get; private set; } = 0.2f;
    [field: SerializeField, Tooltip("Impulse force applied when rolling in the air.")] public float rollAirSpeed { get; private set; } = 15f;
    [field: SerializeField, Tooltip("Duration of the air roll in seconds.")] public float rollAirTime { get; private set; } = 0.2f;
    [field: SerializeField, Tooltip("Cooldown in seconds before the player can roll again.")] public float rollCooldown { get; private set; } = 0.2f;
    [field: SerializeField, Tooltip("How quickly the player reaches max speed.")] public float acceleration { get; private set; } = 80f;
    [field: SerializeField, Tooltip("How quickly the player slows down when no input is given.")] public float deceleration { get; private set; } = 60f;
    [field: SerializeField, Tooltip("How long after walking off a ledge the player can still jump (in seconds).")] public float coyoteTime { get; private set; } = 0.15f;
    [field: SerializeField, Tooltip("How early before landing the player can press jump and still trigger it (in seconds).")] public float jumpBufferTime { get; private set; } = 0.15f;

    [field: Header("Jump Feel")]
    [field: SerializeField, Tooltip("Multiplier applied to upward velocity when jump is released early. Lower = shorter minimum jump height.")] public float jumpCutMultiplier { get; private set; } = 0.4f;
    [field: SerializeField, Tooltip("Gravity multiplier applied while falling. Higher = snappier fall arc.")] public float fallGravityMultiplier { get; private set; } = 2.5f;
    [field: SerializeField, Tooltip("Gravity multiplier at the top of the jump arc. Lower = floatier apex.")] public float apexGravityMultiplier { get; private set; } = 0.5f;
    [field: SerializeField, Tooltip("Vertical speed threshold below which apex gravity kicks in. Higher = larger apex window.")] public float apexThreshold { get; private set; } = 4f;
    [field: SerializeField, Tooltip("Gravity multiplier when holding down while airborne. Higher = faster fast fall.")] public float fastFallMultiplier { get; private set; } = 3.5f;
    [field: SerializeField, Tooltip("Acceleration multiplier applied when changing direction. Higher = snappier turnaround.")] public float turnAccelMultiplier { get; private set; } = 2f;

    [field: Header("Wall Jump Settings")]
    [field: SerializeField, Tooltip("How long the player can wall jump after leaving the wall (in seconds).")] public float wallJumpTime { get; private set; } = 0.2f;
    [field: SerializeField, Tooltip("How long the wall jump controls are locked (player is pushed away from wall).")] public float wallJumpDuration { get; private set; } = 0.4f;
    [field: SerializeField, Tooltip("X = horizontal force, Y = vertical force applied on wall jump.")] public Vector2 wallJumpPower { get; private set; } = new Vector2(8f, 16f);

    [HideInInspector] public bool canJump;
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
    [HideInInspector] public float currentCoyoteTime;
    [HideInInspector] public float currentJumpBufferTime;
    [HideInInspector] public float baseGravityScale { get; private set; }


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        baseGravityScale = rb.gravityScale;
    }

    void Update()
    {
        if (canFlip)
        {
            if (moveDir.x > 0 && !_facingRight) Flip();
            else if (moveDir.x < 0 && _facingRight) Flip();
        }

        bool wasGrounded = isGrounded;

        GroundWallCheck();

        if (wasGrounded && !isGrounded) currentCoyoteTime = coyoteTime;

        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);

        if (!isWallSliding)
        {
            wallJumpCounter = Mathf.Max(0f, wallJumpCounter - Time.deltaTime);
        }

        if (currentRollCooldownTimer > 0) currentRollCooldownTimer -= Time.deltaTime;
        if (currentCoyoteTime >= 0) currentCoyoteTime -= Time.deltaTime;
        if (currentJumpBufferTime >= 0) currentJumpBufferTime -= Time.deltaTime;

        canJump = isGrounded || currentCoyoteTime > 0;
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