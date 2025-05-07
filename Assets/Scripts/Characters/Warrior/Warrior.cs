using UnityEngine;

/// <summary>
/// Warrior class — inherits FighterBase and modifies it's attack behavior.
/// </summary>
public class Warrior : FighterBase
{
    public override void Attack(FighterBase opponent)
    {
        if (statusEffectManager != null && statusEffectManager.IsStunned())
        {
            Debug.Log($"{fighterName} is stunned and cannot attack!");
            return;
        }

        int damage = baseAttackPower + 5;
        opponent.TakeDamage(damage);
        Debug.Log($"{fighterName} attacks {opponent.fighterName} for {damage} damage!");
    }
}