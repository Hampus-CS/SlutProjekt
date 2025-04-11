using UnityEngine;

/// <summary>
/// Warrior class — inherits FighterBase and modifies it's attack behavior.
/// </summary>
public class Warrior : FighterBase
{
    public override void Attack(FighterBase opponent)
    {
        int damage = baseAttackPower + 5;
        Debug.Log($"{fighterName} (Warrior) attacks {opponent.fighterName} for {damage} damage!");
        opponent.TakeDamage(damage);
    }
}
