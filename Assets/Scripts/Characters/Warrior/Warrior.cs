using UnityEngine;

/// <summary>
/// Warrior class — inherits FighterBase and modifies it's attack behavior.
/// </summary>
public class Warrior : FighterBase
{
    [Header("Dash Pierce Settings")]
    public float dashDistance = 3f;
    public float dashSpeed = 10f;
    public float dashCooldown = 5f;
    public float knockbackForce = 5f;
    public float stunDuration = 0.5f;
    private float lastDashTime = -Mathf.Infinity;
    public int dashDamage = 15;

    private StatusEffectManager statusEffectManager;
    private bool isDashing = false;
    private Vector3 dashTarget;

    private void Start()
    {
        statusEffectManager = GetComponent<StatusEffectManager>();
    }

    private void Update()
    {
        if (statusEffectManager.IsStunned()) return;

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastDashTime + dashCooldown && !isDashing)
        {
            StartDashPierce();
        }

        if (isDashing)
        {
            float step = dashSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, dashTarget, step);

            if (Vector3.Distance(transform.position, dashTarget) < 0.1f)
            {
                isDashing = false;
            }
        }
    }

    private void StartDashPierce()
    {
        isDashing = true;
        lastDashTime = Time.time;

        float direction = 1f;
        if (transform.localScale.x < 0)
        {
            direction = -1f;
        }

        dashTarget = transform.position + new Vector3(direction * dashDistance, 0f, 0f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right * direction, dashDistance);
        if (hit.collider != null)
        {
            FighterBase opponent = hit.collider.GetComponent<FighterBase>();
            if (opponent != null && opponent != this)
            {
                opponent.TakeDamage(dashDamage);

                Rigidbody2D opponentRb = opponent.GetComponent<Rigidbody2D>();
                if (opponentRb != null)
                {
                    Vector2 knockbackDirection = new Vector2(direction, 0f);
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
    }

    public override void Attack(FighterBase opponent)
    {
        if (statusEffectManager != null && statusEffectManager.IsStunned())
        {
            Debug.Log($"{fighterName} is stunned and cannot attack!");
            return;
        }

        int damage = baseAttackPower + 5;
        opponent.TakeDamage(damage);
        Debug.Log($"{fighterName} attacks {opponent.fighterName} for {damage} damage!");
    }
}