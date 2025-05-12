using UnityEngine;
using TMPro;

public abstract class FighterBase : MonoBehaviour
{
    [Header("Base Stats")]
    public string fighterName = "Fighter";
    public int baseAttackPower = 10;
    public int currentHealth;
    public int maxHealth = 100;

    [Header("UI Elements")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI manaText;

    [Header("Mana System")]
    public float maxMana = 100f;
    public float currentMana = 100f;
    public float manaRegenerationRate = 5f;

    [Header("Projectile")]
    public GameObject magicBoltPrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    
    public SpriteRenderer SpriteRenderer => GetComponent<SpriteRenderer>();
    public PlayerMove PlayerMove => GetComponent<PlayerMove>();

    protected Animator animator;
    protected StatusEffectManager statusEffectManager;
   
    private void Start()
    {
        animator = GetComponent<Animator>();
        statusEffectManager = GetComponent<StatusEffectManager>();
        
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateHpText();
        UpdateManaText();

        if (statusEffectManager == null)
        {
            Debug.LogError("StatusEffectManager not found on " + gameObject.name);
        }
    }

    private void FixedUpdate()
    {
        RegenerateMana();
        UpdateHpText();
        UpdateManaText();
    }

    protected void ShootProjectile()
    {
        Debug.Log("Shoot");
        if (magicBoltPrefab != null && firePoint != null)
        {
            GameObject bolt = Instantiate(magicBoltPrefab, firePoint.position, firePoint.rotation);

            Rigidbody2D rb = bolt.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                float direction = 1f;
                if (transform.localScale.x < 0)
                {
                    direction = -1f;
                }
                rb.linearVelocity = new Vector2(direction * projectileSpeed, 0f);
            }

            Debug.Log($"{gameObject.name} shoots a magic bolt!");
        }
        else
        {
            Debug.LogError("Magic bolt prefab or firePoint is missing.");
        }
    }

    public virtual void TakeDamage(int amount)
    {
        PlayDamageAnimation();
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHpText();
    }

    public abstract void Attack(FighterBase opponent);

    protected virtual void Die()
    {
        Debug.Log($"{fighterName} has died.");
        if (animator != null)
        {
            PlayDeathAnimation();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void UpdateHpText()
    {
        if (hpText != null)
        {
            hpText.text = "HP: " + currentHealth;
        }
        else
        {
            Debug.LogWarning("hpText is not assigned on " + gameObject.name);
        }
    }

    private void UpdateManaText()
    {
        if (manaText != null)
        {
            manaText.text = "Mana: " + Mathf.FloorToInt(currentMana).ToString();
        }
    }

    private void RegenerateMana()
    {
        if (currentMana < maxMana)
        {
            currentMana += manaRegenerationRate * Time.deltaTime;
            if (currentMana > maxMana)
            {
                currentMana = maxMana;
            }
        }
    }

    protected bool HasEnoughMana(int amount) => currentMana >= amount;

    protected void SpendMana(int amount)
    {
        currentMana -= amount;

        if (currentMana < 0)
        {
            currentMana = 0;
        }
    }

    // Status Effects
    public void ApplyBurn(int totalBurnDamage, float burnDuration)
    {
        statusEffectManager?.ApplyBurn(totalBurnDamage, burnDuration);
    }

    public void ApplyStun(float stunDuration)
    {
        statusEffectManager?.ApplyStun(stunDuration);
    }

    protected void PlayAttackAnimation()
    {
        animator.SetTrigger("AttackTrigger");
    }

    private void PlayDamageAnimation()
    {
        animator.SetTrigger("DamageTrigger");
    }

    private void PlayDeathAnimation()
    {
        animator.SetTrigger("DeathTrigger");
        Destroy(gameObject);
    }
}