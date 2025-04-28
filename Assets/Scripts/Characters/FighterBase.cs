using UnityEngine;
using System.Collections;
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

    // Status effect management
    private bool isBurning = false;
    public bool isStunned = false;
    private Coroutine burnCoroutine;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateHPText();
        UpdateManaText();
    }

    protected virtual void Update()
    {
        RegenerateMana();

        if (hpText != null)
        {
            hpText.text = "HP: " + currentHealth;
        }

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
            if (currentMana > maxMana) currentMana = maxMana;
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

    // Burn effect
    public void ApplyBurn(int totalBurnDamage, float burnDuration)
    {
        if (isBurning)
        {
            StopCoroutine(burnCoroutine);
            isBurning = false;
        }

        burnCoroutine = StartCoroutine(BurnCoroutine(totalBurnDamage, burnDuration));
    }

    private IEnumerator BurnCoroutine(int totalBurnDamage, float burnDuration)
    {
        isBurning = true;
        yield return new WaitForSeconds(1f);

        float damagePerSecond = totalBurnDamage / burnDuration;
        float pendingDamage = 0f;
        float elapsedTime = 0f;

        Debug.Log($"{fighterName} is burning for {totalBurnDamage} damage over {burnDuration} seconds.");

        while (elapsedTime < burnDuration)
        {
            elapsedTime += Time.deltaTime;
            pendingDamage += damagePerSecond * Time.deltaTime;

            if (pendingDamage >= 1f)
            {
                int damageToApply = Mathf.FloorToInt(pendingDamage);
                TakeDamage(damageToApply);
                pendingDamage -= damageToApply;

                Debug.Log($"{fighterName} burned for {damageToApply} damage. Current HP: {currentHealth}");
            }

            yield return null;
        }

        isBurning = false;
        Debug.Log($"{fighterName}'s burn effect ended.");
    }

    // Stun effect
    public void Stun(float duration)
    {
        if (isStunned) return;

        Debug.Log($"{fighterName} is stunned for {duration} seconds.");
        StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        isStunned = true;

        PlayerMove move = GetComponent<PlayerMove>();
        if (move != null)
        {
            move.BlockMovement(true);
        }

        yield return new WaitForSeconds(duration);

        isStunned = false;

        // Unblock movement
        if (move != null)
        {
            move.BlockMovement(false);
        }

        Debug.Log($"{fighterName} is no longer stunned.");
    }
}