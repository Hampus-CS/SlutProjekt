using UnityEngine;
using TMPro;

public class Mage : FighterBase
{
    [Header("Mage Settings")]
    public GameObject fireballPrefab;
    public Transform firePoint;

    [Header("Fireball Settings")]
    public float fireballSpeed = 10f;
    public float fireballCooldown = 5f;
    private float lastFireballTime = -Mathf.Infinity;
    public int fireballCost = 20;

    private StatusEffectManager statusEffectManager;


    private void Start()
    {
        statusEffectManager = GetComponent<StatusEffectManager>();
    }
    protected override void Update()
    {
        if (statusEffectManager.IsStunned()) return;

        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastFireballTime + fireballCooldown)
        {
            if (HasEnoughMana(fireballCost))
            {
                ShootFireball();
                SpendMana(fireballCost);
                lastFireballTime = Time.time;
            }
            else
            {
                Debug.Log("Not enough mana!");
            }
        }
    }

    void ShootFireball()
    {
        if (fireballPrefab != null && firePoint != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

            float direction = 1f;
            if (transform.localScale.x < 0)
            {
                direction = -1f;
            }
            rb.linearVelocity = new Vector2(direction * fireballSpeed, 0f);

            Vector3 scale = fireball.transform.localScale;
            if (direction < 0)
            {
                scale.x = -Mathf.Abs(scale.x);
            }
            else
            {
                scale.x = Mathf.Abs(scale.x);
            }
            fireball.transform.localScale = scale;
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