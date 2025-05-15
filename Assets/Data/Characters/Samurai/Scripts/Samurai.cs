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

    private void Update()
    {
	    if (!IsOwner) return;
	    
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
        
        if (Input.GetMouseButtonDown(0))
        {
            Attack(null);   
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

        // Determine dash direction based on the character's facing direction
        Vector2 dashDirection = transform.localScale.x < 0 ? Vector2.left : Vector2.right;

        // Set the default dash target to be the max distance, using Vector2 for 2D space
        dashTarget = (Vector2)transform.position + dashDirection * dashDistance;

        // Perform a Raycast to check for obstacles and enemies
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dashDirection, dashDistance, LayerMask.GetMask("Ground", "Fighters"));

        if (hit.collider != null)
        {
            // Check if we hit an enemy
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Fighters"))
            {
                FighterBase opponent = hit.collider.GetComponent<FighterBase>();
                if (opponent != null && opponent != this)
                {
                    // Apply damage, knockback, and stun to the opponent
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

                // If there's still remaining distance after hitting the enemy, continue the dash
                float distanceToTravelAfterEnemy = dashDistance - hit.distance;
                if (distanceToTravelAfterEnemy > 0)
                {
                    dashTarget = hit.point + dashDirection * distanceToTravelAfterEnemy;
                }
            }
            // If we hit the ground, stop the dash at the hit point
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                dashTarget = hit.point;
            }
        }

        // Ensure that dashTarget doesn't exceed the max dash distance
        float remainingDistance = Vector2.Distance(transform.position, dashTarget);
        if (remainingDistance > dashDistance)
        {
            dashTarget = (Vector2)transform.position + dashDirection * dashDistance;
        }

        OnAbilityAnimationEnd();
    }

    private void OnAbilityAnimationEnd()
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

        MeleeAttack();
        PlayAttackAnimation();

        Debug.Log($"{fighterName} attacks {opponent.fighterName}");
    }
}