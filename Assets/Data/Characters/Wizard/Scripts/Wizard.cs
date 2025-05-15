using UnityEngine;

public class Wizard : FighterBase
{
    [Header("Wizard Settings")]
    public GameObject lightningBoltPrefab;

    [Header("Lightning Bolt")]
    public float lightningBoltSpeed = 15f;
    public float lightningBoltCooldown = 3f;
    public int lightningBoltCost = 30;
    private float lastLightningBoltTime = -Mathf.Infinity;

    private void Update()
    {
        if (statusEffectManager == null || statusEffectManager.IsStunned()) return;

        // Lightning bolt cooldown and mana check
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastLightningBoltTime + lightningBoltCooldown)
        {
            if (currentMana >= lightningBoltCost)
            {
                ShootLightningBolt();
                SpendMana(lightningBoltCost);
                lastLightningBoltTime = Time.time;
            }
            else
            {
                Debug.Log("Not enough mana!");
            }
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Attack(null);   
        }
    }

    void ShootLightningBolt()
    {
        if (animator != null)
        {
            animator.SetTrigger("AbilityTrigger");
        }

        if (lightningBoltPrefab != null && firePoint != null)
        {
            GameObject lightningBolt = Instantiate(lightningBoltPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = lightningBolt.GetComponent<Rigidbody2D>();

            float direction = transform.localScale.x < 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * lightningBoltSpeed, 0f);

            Vector3 scale = lightningBolt.transform.localScale;

            if (direction > 0)
            {
                scale.x = Mathf.Abs(scale.x);
            }
            else
            {
                scale.x = -Mathf.Abs(scale.x);
            }

            lightningBolt.transform.localScale = scale;
        }
    }

    public override void Attack(FighterBase opponent)
    {
        Debug.Log($"{fighterName} attacks!");
        if (statusEffectManager != null && statusEffectManager.IsStunned())
        {
            Debug.Log($"{fighterName} is stunned and cannot attack!");
            return;
        }

        PlayAttackAnimation();
        Debug.Log($"{fighterName} attacks!");
        ProjectileAttack();
    }
}