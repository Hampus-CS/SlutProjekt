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

	void ShootFireball()
	{
		if (fireballPrefab != null && firePoint != null)
		{
			GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
			Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();

			float direction = transform.localScale.x < 0 ? 1f : -1f;
			rb.linearVelocity = new Vector2(direction * fireballSpeed, 0f);

			Vector3 scale = fireball.transform.localScale;

			if (direction < 0)
			{
				scale.x = -Mathf.Abs(scale.x);
			}
			else
			{
				scale.x = Mathf.Abs(scale.x);
			}

			fireball.transform.localScale = scale;
		}
	}

	public override void Attack(FighterBase opponent)
	{
		Debug.Log($"{fighterName} attacks!");
		if (statusEffectManager != null && statusEffectManager.IsStunned())
		{
			Debug.Log($"{fighterName} is stunned and cannot attack!");
			return;
		}

		PlayAttackAnimation();
		Debug.Log($"{fighterName} attacks!");
		ProjectileAttack();
	}
}