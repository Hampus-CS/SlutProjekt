using TMPro;
using UnityEditor;
using UnityEngine;

public class Wizzard : FighterBase
{
    [Header("Wizzard Settings")]
    public GameObject lightningBoltPrefab;
    public Transform firePoint;

    [Header("Lightning Bolt")]
    public float lightningBoltSpeed = 15f;
    public float lightningBoltCooldown = 3f;
    private float lastLightningBoltTime = -Mathf.Infinity;
    public int lightningBoltCost = 30;

    private StatusEffectManager statusEffectManager;

    private void Start()
    {
        statusEffectManager = GetComponent<StatusEffectManager>();
    }
    private void Update()
    {
        if (statusEffectManager.IsStunned()) return;

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
    }
    void ShootLightningBolt()
    {
        if (lightningBoltPrefab != null && firePoint != null)
        {
            GameObject lightningBolt = Instantiate(lightningBoltPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = lightningBolt.GetComponent<Rigidbody2D>();

            float direction = transform.localScale.x > 0 ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * lightningBoltSpeed, 0f);

            Vector3 scale = lightningBolt.transform.localScale;
            scale.x = direction > 0 ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            lightningBolt.transform.localScale = scale;
        }
    }

    public override void Attack(FighterBase opponent)
    {
        if (statusEffectManager != null && statusEffectManager.IsStunned())
        {
            Debug.Log($"{fighterName} is stunned and cannot attack!");
            return;
        }

        int damage = baseAttackPower + 2;
        opponent.TakeDamage(damage);
    }
}
