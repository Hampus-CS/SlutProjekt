using UnityEngine;
using System.Collections.Generic;

public class StatusEffectManager : MonoBehaviour
{
	private PlayerMove moveScript;
	private FighterBase fighter;
	private List<StatusEffect> activeEffects = new();

	private void Start()
	{
		fighter = GetComponent<FighterBase>();
		moveScript = fighter.GetComponent<PlayerMove>();
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
		BurnEffect burn = new(duration, totalBurnDamage, fighter);
		activeEffects.Add(burn);
		burn.Start();
	}

	public void ApplyStun(float duration)
	{
		StunEffect stun = new(duration, fighter);
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
		private float duration;
		private float elapsed;

		public bool IsFinished => elapsed >= duration;

		protected StatusEffect(float duration, FighterBase fighter)
		{
			this.duration = duration;
			this.fighter = fighter;
			elapsed = 0f;
		}

		public abstract void Start();

		public virtual void UpdateEffect(float deltaTime)
		{
			elapsed += deltaTime;
		}

		public abstract void End();
	}

	// Burn Effect
	private class BurnEffect : StatusEffect
	{
		private float totalBurnDamage;
		private float damagePerSecond;
		private float damageAccumulated;

		private SpriteRenderer spriteRenderer;
		private Color originalColor;
		private float pulseSpeed = 4f;

		public BurnEffect(float duration, int totalBurnDamage, FighterBase fighter) : base(duration, fighter)
		{
			damagePerSecond = totalBurnDamage / duration;
			damageAccumulated = 0f;

			spriteRenderer = fighter.GetComponent<SpriteRenderer>();
			if (spriteRenderer != null)
			{
				originalColor = spriteRenderer.color;
			}
		}

		public override void Start()
		{
			spriteRenderer = fighter.SpriteRenderer;

			if (spriteRenderer != null)
			{
				originalColor = spriteRenderer.color;
			}

			Debug.Log($"{fighter.fighterName} starts burning!");
		}

		public override void UpdateEffect(float deltaTime)
		{
			base.UpdateEffect(deltaTime);
			damageAccumulated += damagePerSecond * deltaTime;

			if (damageAccumulated >= 1f)
			{
				int damageToApply = Mathf.FloorToInt(damageAccumulated);
				fighter.TakeDamage(damageToApply, fighter);
				damageAccumulated -= damageToApply;
				Debug.Log($"{fighter.fighterName} takes {damageToApply} burn damage!");
			}

			if (spriteRenderer != null)
			{
				float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
				Color burnColor = new(1f, 0f, 0f, 0.9f * pulse);
				spriteRenderer.color = Color.Lerp(originalColor, burnColor, pulse);
			}
		}

		public override void End()
		{
			if (spriteRenderer != null)
			{
				spriteRenderer.color = originalColor;
			}

			Debug.Log($"{fighter.fighterName} burn effect ended.");
		}
	}

	// Stun Effect
	private class StunEffect : StatusEffect
	{
		private PlayerMove moveScript;
		private SpriteRenderer spriteRenderer;
		private Color originalColor;
		private Color stunColor = new(0.6f, 0.8f, 1f, 1f);

		public StunEffect(float duration, FighterBase fighter) : base(duration, fighter)
		{
			moveScript = fighter.GetComponent<PlayerMove>();
			spriteRenderer = fighter.GetComponent<SpriteRenderer>();

			if (spriteRenderer != null)
			{
				originalColor = spriteRenderer.color;
			}
		}

		public override void Start()
		{
			moveScript = fighter.PlayerMove;
			spriteRenderer = fighter.SpriteRenderer;

			if (moveScript != null)
			{
				moveScript.BlockMovement(true);
			}

			if (spriteRenderer != null)
			{
				originalColor = spriteRenderer.color;
				spriteRenderer.color = stunColor;
			}

			Debug.Log($"{fighter.fighterName} is stunned!");
		}

		public override void UpdateEffect(float deltaTime)
		{
			base.UpdateEffect(deltaTime);
		}

		public override void End()
		{
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