using UnityEngine;

/// <summary>
/// Warrior class — inherits FighterBase and modifies it's attack behavior.
/// </summary>
public class Warrior : FighterBase
{
	private bool isCritActive;
	private float critTimer;
	public float critDuration = 5f;
	public int critDamageMultiplier = 3;
	private float lastCritTime = -Mathf.Infinity;
	private float critCooldown = 5f;

	private void Update()
	{
		if (!IsOwner) return;
		if (isDead) return;
		if (statusEffectManager.IsStunned()) return;

		if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastCritTime + critCooldown)
		{
			Crit();
		}

		if (isCritActive)
		{
			critTimer -= Time.deltaTime;
			if (critTimer <= 0f)
			{
				isCritActive = false;
				Debug.Log($"{fighterName}'s crit expired!");
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			Attack(null);
		}
	}

	private void Crit()
	{
		isCritActive = true;
		critTimer = critDuration;
		lastCritTime = Time.time;
	}

	public override void Attack(FighterBase opponent)
	{
		// Block if stunned
		if (isDead) return;
		if (statusEffectManager.IsStunned()) return;

		// If no direct opponent passed (local click) -> use normal melee detection
		if (opponent == null)
		{
			MeleeAttack();
			return;
		}

		// Otherwise we’re on a remote call with a valid reference
		PlayAttackAnimation();

		int damage = baseAttackPower + 5;
		if (isCritActive)
		{
			damage *= critDamageMultiplier;
			isCritActive = false;
			Debug.Log($"{fighterName} lands a crit!");
		}

		opponent.TakeDamage(damage, this);
		Debug.Log($"{fighterName} attacks {opponent.fighterName} for {damage} damage!");
	}
}