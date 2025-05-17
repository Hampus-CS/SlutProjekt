using UnityEngine;

public class Wizard : FighterBase
{
	[Header("Wizard Settings")]
	public GameObject lightningBoltPrefab;

	[Header("Lightning Bolt")]
	public float lightningBoltSpeed = 15f;
	public float lightningBoltCooldown = 3f;
	public int lightningBoltCost = 30;
	private float lastLightningBoltTime = -Mathf.Infinity;

	private void Update()
	{
		if (!IsOwner) return;
		if (isDead) return;
		if (statusEffectManager.IsStunned()) return;

		// Lightning bolt cooldown and mana check
		if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastLightningBoltTime + lightningBoltCooldown)
		{
			if (currentMana >= lightningBoltCost)
			{
				ShootLightningBolt();
				SpendMana(lightningBoltCost);
				lastLightningBoltTime = Time.time;
			}
			else
			{
				Debug.Log("Not enough mana!");
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			Attack(null);
		}
	}

	void ShootLightningBolt()
	{
		if (animator != null)
		{
			if (!IsOwner) return;
			animator.SetTrigger("AbilityTrigger");
		}

		if (lightningBoltPrefab != null && firePoint != null)
		{
			GameObject lightningBolt = Instantiate(lightningBoltPrefab, firePoint.position, Quaternion.identity);
			LightningBolt lightningBoltScript = lightningBolt.GetComponent<LightningBolt>();

			if (lightningBoltScript != null)
			{
				lightningBoltScript.attacker = this;
			}
			else
			{
				Debug.LogWarning("lightningBolt Script missing on lightningBolt prefab!");
			}

			Rigidbody2D rb = lightningBolt.GetComponent<Rigidbody2D>();

			float direction = transform.localScale.x < 0 ? -1f : 1f;
			rb.linearVelocity = new Vector2(direction * lightningBoltSpeed, 0f);

			Vector3 scale = lightningBolt.transform.localScale;
			scale.x = Mathf.Abs(scale.x) * direction;
			lightningBolt.transform.localScale = scale;
		}
	}

	public override void Attack(FighterBase opponent)
	{
		Debug.Log($"{fighterName} attacks!");
		if (isDead) return;
		if (statusEffectManager.IsStunned()) return;

		PlayAttackAnimation();
		Debug.Log($"{fighterName} attacks!");
		ProjectileAttack();
	}
}