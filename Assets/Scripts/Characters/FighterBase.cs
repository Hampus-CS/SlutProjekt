using UnityEngine;
using System.Collections;
using TMPro; // If you're using TextMeshPro for HP display

/// <summary>
/// Base class for all fighters (Mage, Warrior, etc.) — handles health, damage, and status effects.
/// </summary>
public abstract class FighterBase : MonoBehaviour
{
    public string fighterName = "Fighter";
    public int baseAttackPower = 10;
    public int currentHealth;
    public int maxHealth = 100;

    [Header("UI Elements")]
    public TextMeshProUGUI hpText;

    // Status effect management
    private bool isBurning = false;
    private Coroutine burnCoroutine;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHPText();
    }

    private void Update()
    {
        if (hpText != null)
        {
            hpText.text = "HP: " + currentHealth;
        }
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

    // Apply a burn effect to the target
    public void ApplyBurn(int totalBurnDamage, float burnDuration, float burnTickInterval)
    {
        if (!isBurning)
        {
            burnCoroutine = StartCoroutine(BurnCoroutine(totalBurnDamage, burnDuration, burnTickInterval));
        }
    }

    // Coroutine to handle burn damage over time
    private IEnumerator BurnCoroutine(int totalBurnDamage, float burnDuration, float burnTickInterval)
    {
        isBurning = true;

        int numberOfTicks = Mathf.CeilToInt(burnDuration / burnTickInterval);
        int damagePerTick = totalBurnDamage / numberOfTicks;

        Debug.Log($"{fighterName} will burn for {damagePerTick} damage every {burnTickInterval}s for {numberOfTicks} ticks.");

        yield return new WaitForSeconds(1f);

        int ticksDone = 0;

        while (ticksDone < numberOfTicks)
        {
            TakeDamage(damagePerTick);
            Debug.Log($"{fighterName} burn tick {ticksDone + 1}/{numberOfTicks}. Current HP: {currentHealth}");

            ticksDone++;

            yield return new WaitForSeconds(burnTickInterval);
        }

        isBurning = false;
        Debug.Log($"{fighterName}'s burn effect ended.");
    }
}