using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float dashSpeed = 10f;
    public float jumpCooldown = 0.2f;
    public float stopDelay = 0.5f;

    private Vector2 moveDirection = Vector2.zero;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    private Rigidbody2D rb;
    private bool IsGrounded;
    private bool IsMoving;

    private float stopTimer = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Ground check
        Vector2 boxSize = new Vector2(0.2f, 0.05f); // width, height
        IsGrounded = Physics2D.OverlapBox(groundCheck.position, boxSize, 0f, groundLayer);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Movement input
        bool movingLeft = Input.GetKey(KeyCode.A);
        bool movingRight = Input.GetKey(KeyCode.D);
        bool both = movingLeft && movingRight;

        if (movingLeft && !both)
        {
            moveDirection = Vector2.left;
            rb.AddForce(moveDirection * moveSpeed);
            IsMoving = true;
            stopTimer = 0f;
        }
        else if (movingRight && !both)
        {
            moveDirection = Vector2.right;
            rb.AddForce(moveDirection * moveSpeed);
            IsMoving = true;
            stopTimer = 0f;
        }
        else if (IsMoving)
        {
            // Fade out movement when not pressing A or D
            stopTimer += Time.fixedDeltaTime;
            float t = stopTimer / stopDelay;
            float fadedX = Mathf.Lerp(rb.linearVelocity.x, 0f, t);
            rb.linearVelocity = new Vector2(fadedX, rb.linearVelocity.y);

            if (t >= 1f)
            {
                IsMoving = false;
                stopTimer = 0f;
            }
        }

        // Dash (only while moving)
        if (Input.GetKeyDown(KeyCode.LeftShift) && moveDirection != Vector2.zero)
        {
            rb.linearVelocity = new Vector2(moveDirection.x * dashSpeed, rb.linearVelocity.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 boxSize = new Vector2(0.2f, 0.05f);
        Gizmos.DrawWireCube(groundCheck.position, boxSize);
    }
}
