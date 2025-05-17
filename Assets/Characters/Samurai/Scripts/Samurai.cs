using System;
using System.Collections;
using UnityEngine;

public class Samurai : FighterBase
{
	[Header("Dash Pierce Settings")]
	private float dashDistance = 10f;
	private float dashDuration = 0.3f;
	private float knockbackForce = 10f;
	private float stunDuration = 0.5f;
	private float lastDashTime;
	private int dashDamage = 10;
	private bool isDashing = false;
	private bool isPerformingAbility = false;
	private bool hasDashedHit;

	[Header("Colliders")]
	private Collider2D mainCollider;
	private BoxCollider2D leftWallCollider;
	private BoxCollider2D rightWallCollider;
	private BoxCollider2D floorCollider;

	private Vector2 dashTarget;

	private Rigidbody2D rb;

	[Header("Ghost afterImage")]
	public GameObject ghostPrefab;
	private GhostSpawner ghostSpawner;
	private float ghostSpawnInterval;
	private float ghostSpawnTimer = 0f;
	private float numberOfGhost = 10f;

	private void Start()
	{
		ghostSpawner = GetComponent<GhostSpawner>();
		rb = GetComponent<Rigidbody2D>();
		rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		animator = GetComponent<Animator>();
		statusEffectManager = GetComponent<StatusEffectManager>();

		// Assign colliders
		leftWallCollider = GameObject.Find("LeftWall")?.GetComponent<BoxCollider2D>();
		rightWallCollider = GameObject.Find("RightWall")?.GetComponent<BoxCollider2D>();
		floorCollider = GameObject.Find("Floor")?.GetComponent<BoxCollider2D>();

		mainCollider = GetComponent<Collider2D>();

		if (leftWallCollider == null || rightWallCollider == null || floorCollider == null)
		{
			Debug.LogError("One or more wall/floor colliders are missing!");
		}
	}

	private void Update()
	{
		if (!IsOwner) return;
		if (isDead) return;
		if (statusEffectManager.IsStunned()) return;

		if (Input.GetKeyDown(KeyCode.Space) && !isPerformingAbility && Time.time - lastDashTime > dashDuration)
		{
			StartDashPierce();
		}
	}

	private void StartDashPierce()
	{
		if (!isDashing)
		{
			hasDashedHit = false;
			StartCoroutine(DashCoroutine());
		}
	}

	private IEnumerator DashCoroutine()
	{
		mainCollider.isTrigger = true;
		isDashing = true;
		isPerformingAbility = true;
		lastDashTime = Time.time;
		hasDashedHit = false;

		if (animator != null)
			animator.SetTrigger("AbilityTrigger");

		gameObject.layer = LayerMask.NameToLayer("NoCollision");
		if (rb != null)
			rb.gravityScale = 0f;

		Vector2 dashDirection = transform.localScale.x < 0 ? Vector2.left : Vector2.right;
		dashTarget = (Vector2)transform.position + dashDirection * dashDistance;

		float minX = leftWallCollider.bounds.max.x;
		float maxX = rightWallCollider.bounds.min.x;
		float minY = floorCollider.bounds.max.y;

		dashTarget.x = Mathf.Clamp(dashTarget.x, minX, maxX);
		dashTarget.y = Mathf.Max(dashTarget.y, minY);

		float elapsed = 0f;
		ghostSpawnInterval = dashDuration / numberOfGhost;

		while (elapsed < dashDuration)
		{
			Vector2 step = dashDirection * ((dashDistance / dashDuration) * Time.deltaTime);
			rb.MovePosition(rb.position + step);

			ghostSpawnTimer += Time.deltaTime;
			if (ghostSpawner != null && ghostSpawnTimer >= ghostSpawnInterval)
			{
				SpriteRenderer sr = GetComponent<SpriteRenderer>();
				ghostSpawner.SpawnGhost(transform.position, sr, transform.localScale);
				ghostSpawnTimer = 0f;
			}

			if (!hasDashedHit)
			{
				RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.5f, dashDirection, 0.1f, LayerMask.GetMask("Fighters"));
				if (hit.collider != null)
				{
					FighterBase opponent = hit.collider.GetComponent<FighterBase>();
					if (opponent != null && opponent != this)
					{
						hasDashedHit = true;
						opponent.TakeDamage(dashDamage, this);

						Rigidbody2D opponentRb = opponent.GetComponent<Rigidbody2D>();
						if (opponentRb != null)
						{
							Vector2 knockbackDirection = new Vector2(dashDirection.x, 0f);
							opponentRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
						}

						opponent.GetComponent<StatusEffectManager>()?.ApplyStun(stunDuration);
						Debug.Log($"{fighterName} performs Dash Pierce on {opponent.fighterName} for {dashDamage} damage!");
					}
				}
			}

			RaycastHit2D wallHit = Physics2D.Raycast(transform.position, dashDirection, 0.1f, LayerMask.GetMask("Ground"));
			if (wallHit.collider != null &&
			    (wallHit.collider == leftWallCollider || wallHit.collider == rightWallCollider || wallHit.collider == floorCollider))
			{
				Debug.Log("Dash stopped by wall.");
				break;
			}

			elapsed += Time.deltaTime;
			yield return null;
		}

		gameObject.layer = LayerMask.NameToLayer("Fighters");
		if (rb != null) rb.gravityScale = 1f;
		isDashing = false;
		isPerformingAbility = false;
		mainCollider.isTrigger = false;
	}

	private void OnCollisionEnter2D(Collision2D collider)
	{
		if (!isDashing) return;

		if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
			isDashing = false;
	}

	public override void Attack(FighterBase opponent)
	{
		if (isDead) return;
		if (statusEffectManager == null || statusEffectManager.IsStunned()) return;

		int damage = baseAttackPower + 5;
		PlayAttackAnimation();
		MeleeAttack();
		opponent.TakeDamage(damage, this);
		Debug.Log($"{fighterName} attacks {opponent.fighterName} for {damage} damage!");
	}
}