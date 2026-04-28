using System;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float groundRadius;
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float wallRadius;
    [SerializeField] private Transform wallCheckPos;
    [SerializeField] private LayerMask wallLayer;

    [HideInInspector] public bool canFlip = true;
    [field: SerializeField] public float walkSpeed { get; private set; }
    [field: SerializeField] public float runSpeed { get; private set; }
    [field: SerializeField] public float jumpForce { get; private set; }
    [field: SerializeField] public float wallSlideSpeed { get; private set; }
    public bool isWallJumping;
    public float wallJumpDir;
    [field: SerializeField] public float wallJumpTime { get; private set; } = 0.2f;
    public float wallJumpCounter;
    [field: SerializeField] public float wallJumpDuration { get; private set; } = 0.4f;
    [field: SerializeField] public Vector2 wallJumpPower { get; private set; } = new Vector2(8f, 16f);
    public bool isGrounded { get; private set; }
    public bool isOnWall { get; private set; }

    public Rigidbody2D rb { get; private set; }
    public Animator animator { get; private set; }

    public Vector2 moveDir;
    public bool isSprinting;
    public bool isWallSliding;

    public bool _facingRight = true;

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
            wallJumpCounter -= Time.deltaTime;
        }
    }

    void GroundWallCheck()
    {
        Collider2D groundHits = Physics2D.OverlapCircle(groundCheckPos.position, groundRadius, groundLayer);
        Collider2D wallHits = Physics2D.OverlapCircle(wallCheckPos.position, wallRadius, wallLayer);
        isGrounded = groundHits != null;
        isOnWall = wallHits != null;

        Debug.Log("[Checking Wall] > " + wallHits != null);
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
