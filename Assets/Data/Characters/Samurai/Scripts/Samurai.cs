using UnityEngine;

public class Samurai : FighterBase
{
    [Header("Dash Pierce Settings")]
    public float dashDistance = 5f;
    public float dashSpeed = 10f;
    public float dashCooldown = 5f;
    public float knockbackForce = 5f;
    public float stunDuration = 0.5f;
    public int dashDamage = 15;

    private float lastDashTime = -Mathf.Infinity;
    private bool isDashing;
    private bool isPerformingAbility;
    private Vector3 dashTarget;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        if (statusEffectManager.IsStunned() || isPerformingAbility) return;

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastDashTime + dashCooldown && !isDashing &&
            !isPerformingAbility)
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
        isPerformingAbility = true;
        lastDashTime = Time.time;

        // Trigger animation
        if (animator != null)
        {
            animator.SetTrigger("Ability");
        }

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

    public void OnAbilityAnimationEnd()
    {
        isPerformingAbility = false;
        Debug.Log("Ability animation finished.");
    }

    public override void Attack(FighterBase opponent)
    {
        if (statusEffectManager != null && statusEffectManager.IsStunned())
        {
            Debug.Log($"{fighterName} is stunned and cannot attack!");
            return;
        }

        PlayAttackAnimation();

        int damage = baseAttackPower + 5;
        opponent.TakeDamage(damage);
        Debug.Log($"{fighterName} attacks {opponent.fighterName} for {damage} damage!");
    }
}