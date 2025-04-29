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
    public float currentMana;
    public float manaRegenerationRate = 5f;

    private StatusEffectManager statusEffectManager;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateHPText();
        UpdateManaText();

        statusEffectManager = GetComponent<StatusEffectManager>();
        if (statusEffectManager == null)
        {
            Debug.LogError("StatusEffectManager not found on " + gameObject.name);
        }
    }

    protected virtual void Update()
    {
        RegenerateMana();
        UpdateHPText();
        UpdateManaText();
    }

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHPText();
    }

    public abstract void Attack(FighterBase opponent);

    protected virtual void Die()
    {
        Debug.Log($"{fighterName} has died.");
        Destroy(gameObject);
    }

    private void UpdateHPText()
    {
        if (hpText != null)
        {
            hpText.text = "HP: " + currentHealth;
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

    public bool HasEnoughMana(int amount)
    {
        return currentMana >= amount;
    }

    public void SpendMana(int amount)
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
        if (statusEffectManager != null)
        {
            statusEffectManager.ApplyBurn(totalBurnDamage, burnDuration);
        }
    }

    public void ApplyStun(float stunDuration)
    {
        if (statusEffectManager != null)
        {
            statusEffectManager.ApplyStun(stunDuration);
        }
    }
}