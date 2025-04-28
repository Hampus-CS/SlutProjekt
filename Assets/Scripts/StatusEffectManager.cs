using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StatusEffectManager : MonoBehaviour
{
    private FighterBase fighter;
    private List<StatusEffect> activeEffects = new List<StatusEffect>();

    private void Awake()
    {
        fighter = GetComponent<FighterBase>();
    }

    private void Update()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].UpdateEffect(Time.deltaTime);

            if (activeEffects[i].IsFinished)
            {
                activeEffects[i].End();
                activeEffects.RemoveAt(i);
            }
        }
    }

    public void ApplyBurn(int totalBurnDamage, float duration)
    {
        BurnEffect burn = new BurnEffect(duration, totalBurnDamage, fighter);
        activeEffects.Add(burn);
        burn.Start();
    }

    public void ApplyStun(float duration)
    {
        StunEffect stun = new StunEffect(duration, fighter);
        activeEffects.Add(stun);
        stun.Start();
    }

    public bool IsStunned()
    {
        foreach (var effect in activeEffects)
        {
            if (effect is StunEffect)
                return true;
        }
        return false;
    }

    private abstract class StatusEffect
    {
        protected FighterBase fighter;
        protected float duration;
        protected float elapsed;

        public bool IsFinished => elapsed >= duration;

        public StatusEffect(float duration, FighterBase fighter)
        {
            this.duration = duration;
            this.fighter = fighter;
            elapsed = 0f;
        }

        public abstract void Start();
        public abstract void UpdateEffect(float deltaTime);
        public abstract void End();
    }

    // Burn Effect
    private class BurnEffect : StatusEffect
    {
        private float totalBurnDamage;
        private float damagePerSecond;
        private float damageAccumulated;

        public BurnEffect(float duration, int totalBurnDamage, FighterBase fighter) : base(duration, fighter)
        {
            this.totalBurnDamage = totalBurnDamage;
            this.damagePerSecond = totalBurnDamage / duration;
            this.damageAccumulated = 0f;
        }

        public override void Start()
        {
            Debug.Log($"{fighter.fighterName} starts burning!");
        }

        public override void UpdateEffect(float deltaTime)
        {
            elapsed += deltaTime;
            damageAccumulated += damagePerSecond * deltaTime;

            if (damageAccumulated >= 1f)
            {
                int damageToApply = Mathf.FloorToInt(damageAccumulated);
                fighter.TakeDamage(damageToApply);
                damageAccumulated -= damageToApply;
                Debug.Log($"{fighter.fighterName} takes {damageToApply} burn damage!");
            }
        }

        public override void End()
        {
            Debug.Log($"{fighter.fighterName} burn effect ended.");
        }
    }

    // Stun Effect
    private class StunEffect : StatusEffect
    {
        private PlayerMove moveScript;
        private SpriteRenderer spriteRenderer;
        private Color originalColor;

        public StunEffect(float duration, FighterBase fighter) : base(duration, fighter) { }

        public override void Start()
        {
            moveScript = fighter.GetComponent<PlayerMove>();
            spriteRenderer = fighter.GetComponent<SpriteRenderer>();

            // Block movement and change color
            if (moveScript != null)
            {
                moveScript.BlockMovement(true);
            }

            if (spriteRenderer != null)
            {
                originalColor = spriteRenderer.color;
                spriteRenderer.color = Color.blue;
            }

            Debug.Log($"{fighter.fighterName} is stunned!");
        }

        public override void UpdateEffect(float deltaTime)
        {
            elapsed += deltaTime;
        }

        public override void End()
        {
            // Reset color and unblock movement
            if (moveScript != null)
            {
                moveScript.BlockMovement(false);
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.color = originalColor;
            }

            Debug.Log($"{fighter.fighterName} stun effect ended.");
        }
    }


}
