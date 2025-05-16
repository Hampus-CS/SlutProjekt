using System;
using System.Collections;
using UnityEngine;

public class Samurai : FighterBase
{
	[Header("Dash Pierce Settings")] public float dashDistance = 5f;
	public float dashDuration = 0.3f;
	public int dashDamage = 10;
	public float knockbackForce = 10f;
	public float stunDuration = 1f;
	public float lastDashTime;
	private bool isDashing = false;
	private bool isPerformingAbility = false;

	private BoxCollider2D leftWallCollider;
	private BoxCollider2D rightWallCollider;
	private BoxCollider2D floorCollider;
	
	private Vector2 dashTarget;

	private Rigidbody2D rb;
	
	public GameObject ghostPrefab;
	private GhostSpawner ghostSpawner;

	private void Start()
	{
		ghostSpawner = GetComponent<GhostSpawner>();
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		
		// Auto-assign colliders from scene
		leftWallCollider = GameObject.Find("LeftWall")?.GetComponent<BoxCollider2D>();
		rightWallCollider = GameObject.Find("RightWall")?.GetComponent<BoxCollider2D>();
		floorCollider = GameObject.Find("Floor")?.GetComponent<BoxCollider2D>();

		if (leftWallCollider == null || rightWallCollider == null || floorCollider == null)
		{
			Debug.LogError("One or more wall/floor colliders are missing!");
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space) && !isPerformingAbility && Time.time - lastDashTime > dashDuration)
		{
			StartDashPierce();
		}
	}

	private void StartDashPierce()
	{
		if (!isDashing)
		{
			StartCoroutine(DashCoroutine());
		}
	}

	private IEnumerator DashCoroutine()
{
    isDashing = true;
    isPerformingAbility = true;
    lastDashTime = Time.time;

    if (animator != null)
        animator.SetTrigger("Ability");

    // Change layer so player can pass through enemies
    gameObject.layer = LayerMask.NameToLayer("NoCollision");

    if (rb != null)
        rb.gravityScale = 0f;

    Vector2 dashDirection = transform.localScale.x < 0 ? Vector2.left : Vector2.right;
    dashTarget = (Vector2)transform.position + dashDirection * dashDistance;

    // Clamp dashTarget inside arena bounds
    float minX = leftWallCollider.bounds.max.x;
    float maxX = rightWallCollider.bounds.min.x;
    float minY = floorCollider.bounds.max.y;

    dashTarget.x = Mathf.Clamp(dashTarget.x, minX, maxX);
    dashTarget.y = Mathf.Max(dashTarget.y, minY);

    float elapsed = 0f;

    while (elapsed < dashDuration)
    {
        transform.position = Vector2.MoveTowards(transform.position, dashTarget, (dashDistance / dashDuration) * Time.deltaTime);
	    // Spawn ghost trail here
	    if (ghostSpawner != null)
        {
	        SpriteRenderer sr = GetComponent<SpriteRenderer>();
	        ghostSpawner.SpawnGhost(transform.position, sr);
        }
        // Damage enemies on dash path
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.5f, dashDirection, 0.1f,
            LayerMask.GetMask("Fighters"));

        if (hit.collider != null)
        {
            FighterBase opponent = hit.collider.GetComponent<FighterBase>();
            if (opponent != null && opponent != this)
            {
                opponent.TakeDamage(dashDamage, this);

                Rigidbody2D opponentRb = opponent.GetComponent<Rigidbody2D>();
                if (opponentRb != null)
                {
                    Vector2 knockbackDirection = new Vector2(dashDirection.x, 0f);
                    opponentRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }

                StatusEffectManager opponentStatus = opponent.GetComponent<StatusEffectManager>();
                if (opponentStatus != null)
                {
                    opponentStatus.ApplyStun(stunDuration);
                }

                Debug.Log($"{fighterName} performs Dash Pierce on {opponent.fighterName} for {dashDamage} damage!");
            }
        }

        // Check for collision with walls/floor to stop dash
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, dashDirection, 0.1f, LayerMask.GetMask("Ground"));

        if (wallHit.collider != null &&
            (wallHit.collider == leftWallCollider || wallHit.collider == rightWallCollider || wallHit.collider == floorCollider))
        {
            Debug.Log("Dash stopped by wall or floor.");
            break;
        }

        elapsed += Time.deltaTime;
        yield return null;
    }

    // Reset layer and gravity
    gameObject.layer = LayerMask.NameToLayer("Fighters");

    if (rb != null)
        rb.gravityScale = 1f;

    isDashing = false;
    isPerformingAbility = false;
}

	public override void Attack(FighterBase opponent)
	{
		if (statusEffectManager != null && statusEffectManager.IsStunned())
		{
			Debug.Log($"{fighterName} is stunned and cannot attack!");
			return;
		}

		int damage = baseAttackPower + 5;
		PlayAttackAnimation();
		MeleeAttack();
		opponent.TakeDamage(damage, this);
		Debug.Log($"{fighterName} attacks {opponent.fighterName} for {damage} damage!");
	}
}