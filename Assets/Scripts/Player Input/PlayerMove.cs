using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 60f;
    public float deceleration = 40f;
    public float velocityPower = 0.95f;
    public float jumpForce = 8f;
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float stopThreshold = 0.05f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isDashing;
    private float dashTimer;

    private bool movingLeft;
    private bool movingRight;

    private Vector2 moveDirection = Vector2.zero;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isDashing) return;

        movingLeft = Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);
        movingRight = Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

        // Handle movement direction
        if (movingLeft && movingRight)
        {
            moveDirection = Vector2.zero; // Stop if both keys are pressed
        }
        else if (movingLeft)
        {
            moveDirection = Vector2.left;
        }
        else if (movingRight)
        {
            moveDirection = Vector2.right;
        }
        else
        {
            moveDirection = Vector2.zero; // No movement if neither is pressed
        }

        // Jump (only if grounded)
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Dash (only if there is movement direction)
        if (Input.GetKeyDown(KeyCode.LeftShift) && moveDirection != Vector2.zero)
        {
            StartDash();
        }

        
        if (movingLeft)
        {
            FlipPlayer(true);
        }
        else if (movingRight)
        {
            FlipPlayer(false);
        }
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isDashing)
        {
            HandleDash();
            return;
        }

        HandleMovement();
    }

    private void HandleMovement()
    {
        // Calculate target speed based on move direction and speed
        float targetSpeed = moveDirection.x * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;

        // Use acceleration if moving, deceleration if stopping
        float accelRate = moveDirection != Vector2.zero ? acceleration : deceleration;

        // Smooth movement with a velocity curve
        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velocityPower) * Mathf.Sign(speedDiff);

        // Apply movement to the Rigidbody2D
        rb.linearVelocity = new Vector2(rb.linearVelocity.x + movement * Time.fixedDeltaTime, rb.linearVelocity.y); 

        // If velocity is close to zero and no movement direction, snap to 0 (stop)
        if (Mathf.Abs(rb.linearVelocity.x) < stopThreshold && moveDirection == Vector2.zero) 
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;

        rb.linearVelocity = new Vector2(moveDirection.x * dashSpeed, 0f);
    }

    private void HandleDash()
    {
        dashTimer -= Time.fixedDeltaTime;
        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }

    private void FlipPlayer(bool flipLeft)
    {
        // Flip the entire player's transform based on movement direction
        Vector3 scale = transform.localScale;

        if (flipLeft)
        {
            scale.x = -Mathf.Abs(scale.x); // Flip to the left
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);  // Flip to the right
        }

        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
