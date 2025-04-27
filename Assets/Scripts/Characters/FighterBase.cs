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
    public void ApplyBurn(int totalBurnDamage, float burnDuration)
    {
        if (isBurning)
        {
            StopCoroutine(burnCoroutine);
            isBurning = false;
        }

        burnCoroutine = StartCoroutine(BurnCoroutine(totalBurnDamage, burnDuration));
    }

    // Coroutine to handle burn damage over time
    private IEnumerator BurnCoroutine(int totalBurnDamage, float burnDuration)
    {
        isBurning = true;

        yield return new WaitForSeconds(1f);

        float damagePerSecond = totalBurnDamage / burnDuration;

        // Track the damage that will be applied
        float pendingDamage = 0f;

        // Keep track of how long the burn lasts
        float elapsedTime = 0f;

        Debug.Log($"{fighterName} is burning for {totalBurnDamage} damage over {burnDuration} seconds.");

        // Smooth burn damage applied over burn duration
        while (elapsedTime < burnDuration)
        {
            elapsedTime += Time.deltaTime;

            // Accumulate burn damage based on time passed
            pendingDamage += damagePerSecond * Time.deltaTime;

            // Apply the accumulated damage when it reaches 1 or more, rounded to the nearest integer
            if (pendingDamage >= 1f)
            {
                int damageToApply = Mathf.FloorToInt(pendingDamage); // Rounded down to avoid dealing more damage
                TakeDamage(damageToApply);
                pendingDamage -= damageToApply; // Reset accumulated damage after applying

                Debug.Log($"{fighterName} burned for {damageToApply} damage. Current HP: {currentHealth}");
            }

            yield return null; // Wait until the next frame
        }
        
        isBurning = false;
        Debug.Log($"{fighterName}'s smooth burn effect ended.");
    }
}