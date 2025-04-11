using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Mage class — inherits FighterBase and uses magic-based attacks.
/// </summary>
public class Mage : FighterBase
{
    public override void Attack(FighterBase opponent)
    {
        int damage = baseAttackPower + 2; // Less than Warrior but maybe could be a ranged attack later or something like that. But for now 
        Debug.Log($"{fighterName} (Mage) casts a fireball at {opponent.fighterName}, dealing {damage} magic damage!");
        opponent.TakeDamage(damage);
    }
}

// Thoughts about what we can add if we want/have time: Mana systems, Spell cooldowns, AOE or ranged targeting, Status effects (like burn, freeze, etc.)