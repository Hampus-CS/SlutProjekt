using UnityEngine;

/// <summary>
/// Warrior class — inherits FighterBase and modifies it's attack behavior.
/// </summary>
public class Warrior : FighterBase
{
    private bool isCritActive;
    private float critTimer;
    public float critDuration = 5f;
    
    private void Update()
    {
        if(statusEffectManager.IsStunned()) return;
        
        if(Input.GetKeyDown((KeyCode.Space)))
        {
            Crit();
        }

        if (isCritActive)
        {
            critTimer-=Time.deltaTime;
            if (critTimer <= 0f)
            {
                isCritActive = false;
                Debug.Log($"{fighterName}'s crit expired!");
            }
        }
    }
    
    private void Crit()
    {
        isCritActive = true;   
        critTimer = critDuration;
    }
    
    public override void Attack(FighterBase opponent)
    {
        if (statusEffectManager != null && statusEffectManager.IsStunned())
        {
            Debug.Log($"{fighterName} is stunned and cannot attack!");
            return;
        }

        PlayAttackAnimation();

        int damage = baseAttackPower + 5;
        if (isCritActive)
        {
            damage *= 3;
            isCritActive = false;
            Debug.Log($"{fighterName} lands a crit!");
        }
        opponent.TakeDamage(damage);
        Debug.Log($"{fighterName} attacks {opponent.fighterName} for {damage} damage!");
    }
}