using UnityEngine;

/// <summary>
/// Abstract base class for all fighter characters, to make implementing new fighters easier in the future. (Hopefully)
/// </summary>
public abstract class FighterBase : MonoBehaviour
{
    [Header("Stats")]
    public string fighterName = "Unnamed Fighter";
    public int maxHealth = 100;
    public int baseAttackPower = 10;

    [HideInInspector] public int currentHealth;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    public abstract void Attack(FighterBase opponent);

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{fighterName} took {damage} damage. Remaining HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{fighterName} has died!");
        // TODO: animations, effects, disable controls etc.
    }
}
