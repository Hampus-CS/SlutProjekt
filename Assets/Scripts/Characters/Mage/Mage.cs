using UnityEngine;
using TMPro;

/// <summary>
/// Mage class — inherits FighterBase and uses magic-based attacks.
/// </summary>
public class Mage : FighterBase
{
    [Header("Mage Settings")]
    public GameObject fireballPrefab;
    public Transform firePoint;

    [Header("Fireball")]
    public float fireballSpeed = 10f;
    public float fireballCooldown = 5f;
    private float lastFireballTime = -Mathf.Infinity;

    [Header("Mana System")]
    private float maxMana = 100f;
    private float currentMana;
    public int fireballCost = 20;
    public float manaRegenerationRate = 5f;
    public TextMeshProUGUI manaText; // temp for testing, preferably a UI slider or something

    private void Start()
    {
        currentMana = maxMana;
    }

    private void Update()
    {
        // Recharge mana slowly over time
        if (currentMana < maxMana)
        {
            currentMana += manaRegenerationRate * Time.deltaTime;
            if (currentMana > maxMana) currentMana = maxMana;
        }

        // Fireball cooldown and mana check
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastFireballTime + fireballCooldown)
        {
            if (currentMana >= fireballCost)
            {
                ShootFireball();
                currentMana -= fireballCost;
                lastFireballTime = Time.time;
            }
            else
            {
                Debug.Log("Not enough mana!");
            }
        }

        if (manaText != null)
        {
            manaText.text = "Mana: " + Mathf.FloorToInt(currentMana).ToString();  // Display current mana
        }
    }

    void ShootFireball()
    {
        if (fireballPrefab != null && firePoint != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

            Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

            // Determine the direction the fireball should go based on the player's facing direction
            float direction;

            if (transform.localScale.x > 0)
            {
                direction = 1f;
            }
            else
            {
                direction = -1f;
            }

            rb.linearVelocity = new Vector2(direction * fireballSpeed, 0f);

            // Flip the fireball sprite based on direction
            Vector3 scale = fireball.transform.localScale;
            if (direction > 0)
            {
                scale.x = Mathf.Abs(scale.x);
            }
            else
            {
                scale.x = -Mathf.Abs(scale.x);
            }
            fireball.transform.localScale = scale;
        }
    }
    
    public override void Attack(FighterBase opponent)
    {
        int damage = baseAttackPower + 2; // Less than Warrior but maybe could be a ranged attack later or something like that. But for now 
        Debug.Log($"{fighterName} (Mage) casts a fireball at {opponent.fighterName}, dealing {damage} magic damage!");
        opponent.TakeDamage(damage);
    }
    
}

// Thoughts about what we can add if we want/have time: Mana systems, Spell cooldowns, AOE or ranged targeting, Status effects (like burn, freeze, etc.)