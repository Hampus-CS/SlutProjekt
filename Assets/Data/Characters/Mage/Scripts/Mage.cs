using UnityEngine;

public class Mage : FighterBase
{
	[Header("Fireball Settings")]
	public GameObject fireballPrefab;
	public float fireballSpeed = 10f;
	public float fireballCooldown = 5f;
	public int fireballCost = 20;
	private float lastFireballTime = -Mathf.Infinity;

	void Update()
	{
		if (!IsOwner) return;
		if (isDead) return;
		if (statusEffectManager.IsStunned()) return;

		if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastFireballTime + fireballCooldown)
		{
			if (HasEnoughMana(fireballCost))
			{
				ShootFireball();
				SpendMana(fireballCost);
				lastFireballTime = Time.time;
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

	private void ShootFireball()
	{
		if (fireballPrefab != null && firePoint != null)
		{
			GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
			Fireball fireballScript = fireball.GetComponent<Fireball>();

			if (fireballScript != null)
			{
				fireballScript.attacker = this;
			}
			else
			{
				Debug.LogWarning("Fireball script missing on fireball prefab!");
			}

			Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

			float direction = transform.localScale.x < 0 ? -1f : 1f;
			rb.linearVelocity = new Vector2(direction * fireballSpeed, 0f);

			Vector3 scale = fireball.transform.localScale;
			scale.x = Mathf.Abs(scale.x) * direction;
			fireball.transform.localScale = scale;
		}
	}

	public override void Attack(FighterBase opponent)
	{
		Debug.Log($"{fighterName} attacks!");
		if (isDead) return;
		if (statusEffectManager == null || statusEffectManager.IsStunned()) return;

		PlayAttackAnimation();
		Debug.Log($"{fighterName} attacks!");
		ProjectileAttack();
	}
}