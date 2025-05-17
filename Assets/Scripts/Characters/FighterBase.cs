using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(StatusEffectManager))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public abstract class FighterBase : NetworkBehaviour
{
	[Header("Base Stats")]
	public string fighterName = "Fighter";
	public int baseAttackPower = 10;
	public int currentHealth;
	public int maxHealth = 100;

	[Header("UI Elements")]
	public Slider healthSlider;
	public Slider manaSlider;
	private Sliders sliderUI;

	[Header("Mana System")]
	public float maxMana = 100f;
	public float currentMana = 100f;
	public float manaRegenerationRate = 5f;

	[Header("Projectile")]
	public GameObject magicBoltPrefab;
	public Transform firePoint;
	public float projectileSpeed = 10f;
	public float projectileCooldown = 1f;
	private float lastProjectileTime = -Mathf.Infinity;

	[Header("Melee Attack")]
	public float meleeRange = 1.5f;
	public int meleeDamage = 10;
	public LayerMask enemyLayers;
	public Transform attackPoint;
	private float meleeAttackCooldown = 1f;
	private float lastMeleeTime = -Mathf.Infinity;

	// Session stats (local only)
	public static int sessionKills = 0;
	public static int sessionDeaths = 0;

	public SpriteRenderer SpriteRenderer => GetComponent<SpriteRenderer>();
	public PlayerMove PlayerMove => GetComponent<PlayerMove>();

	protected Animator animator;
	protected StatusEffectManager statusEffectManager;

	private void Start()
	{
		var netObj = GetComponent<NetworkObject>();
		if (netObj != null && netObj.IsOwner)
		{
			healthSlider = HUDManager.Instance.HealthSlider;
			manaSlider = HUDManager.Instance.ManaSlider;
		}

		if (IsOwner)
			sliderUI = FindFirstObjectByType<Sliders>();
		animator = GetComponent<Animator>();
		statusEffectManager = GetComponent<StatusEffectManager>();

		currentHealth = maxHealth;
		currentMana = maxMana;

		if (healthSlider != null)
		{
			healthSlider.maxValue = maxHealth;
			healthSlider.value = currentHealth;
		}

		if (manaSlider != null)
		{
			manaSlider.maxValue = maxMana;
			manaSlider.value = currentMana;
		}

		UpdateHealthSlider();
		UpdateManaSlider();

		if (statusEffectManager == null)
		{
			Debug.LogError("StatusEffectManager not found on " + gameObject.name);
		}
	}

	private void FixedUpdate()
	{
		RegenerateMana();
		if (IsOwner)
			UpdateManaSlider();
	}

	public void TakeDamage(int amount, FighterBase attacker)
	{
		PlayDamageAnimation();
		currentHealth -= amount;
		currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

		if (currentHealth <= 0)
		{
			currentHealth = 0;

			if (attacker != null && attacker.IsOwner)
			{
				sessionKills++;
				Debug.Log($"[FighterBase] {attacker.fighterName} gains a kill!");
			}

			Die();
		}

		if (IsOwner && sliderUI != null)
		{
			UpdateHealthSlider();
			sliderUI.FlashHealthOnDamage();
		}
	}

	public abstract void Attack(FighterBase opponent);

	// Ranged characters default attack
	protected void ProjectileAttack()
	{
		if (Time.time < lastProjectileTime + projectileCooldown)
		{
			Debug.Log("Projectile is on cooldown.");
			return;
		}

		Debug.Log("Shoot");
		if (magicBoltPrefab != null && firePoint != null)
		{
			GameObject bolt = Instantiate(magicBoltPrefab, firePoint.position, Quaternion.Euler(0, 0, 90));

			Rigidbody2D rb = bolt.GetComponent<Rigidbody2D>();

			if (rb != null)
			{
				float direction = transform.localScale.x < 0 ? -1f : 1f;
				rb.linearVelocity = new Vector2(direction * projectileSpeed, 0f);
			}

			Debug.Log($"{gameObject.name} shoots a magic bolt!");
			lastProjectileTime = Time.time;
		}
		else
		{
			Debug.LogError("Magic bolt prefab or firePoint is missing.");
		}
	}

	protected void MeleeAttack()
	{
		if (Time.time < lastMeleeTime + meleeAttackCooldown)
		{
			Debug.Log("Melee Attack is on cooldown.");
			return;
		}

		// Detect enemies within melee range
		Collider2D[] enemiesHit = Physics2D.OverlapCircleAll(attackPoint.position, meleeRange, enemyLayers);

		foreach (var enemy in enemiesHit)
		{
			FighterBase enemyFighter = enemy.GetComponent<FighterBase>();
			if (enemyFighter != null)
			{
				Debug.Log($"{gameObject.name} hits {enemyFighter.fighterName} with melee attack!");
				if (enemyFighter.currentHealth == 0)
				{
					var net = GetComponent<NetworkObject>();
				}

				enemyFighter.TakeDamage(meleeDamage, this);
			}
		}

		PlayAttackAnimation();
	}

	private void Die()
	{
		Debug.Log($"{fighterName} has died.");

		if (animator != null)
		{
			animator.SetBool("isDead", true);
			PlayDeathAnimation();

			if (statusEffectManager != null)
			{
				statusEffectManager.IsStunned();
			}
			else
			{
				Debug.LogWarning("statusEffectManager is null");
			}
		}

		if (!IsOwner)
			return; // Only the local player’s object runs the below

		// 1) Record “I died” on my profile
		bool iWon = false;
		int myKills = sessionKills;
		int myDeaths = sessionDeaths + 1;
		
		Debug.Log("Die() has been called and now will attempt to FinalizeLocalMatch()");
		
		FinalizeLocalMatch(iWon, myKills, myDeaths);

		// 2) Tell the opponent to record “I won” on THEIR profile
		if (IsServer)
		{
			Debug.Log("Die() tries to call NotifyOpponentWonClientRpc();");
			// Host dying → directly send ClientRpc to the other client(s)
			NotifyOpponentWonClientRpc();
		}
		else
		{
			Debug.Log("Die() tries to call SubmitOpponentWonServerRpc();");
			// Client dying → asks the host to relay that RPC
			SubmitOpponentWonServerRpc();
		}
		
	}

	private void UpdateHealthSlider()
	{
		if (!IsOwner || healthSlider == null)
			return;
		
		if(healthSlider != null)
		{
			healthSlider.maxValue = maxHealth;
			healthSlider.value = currentHealth;
		}
		else
			Debug.LogWarning("hpSlider is not assigned on " + gameObject.name);	
	}

	private void UpdateManaSlider()
	{
		if (!IsOwner || manaSlider == null)
			return;
		
		if (manaSlider != null)
		{
			manaSlider.maxValue = maxMana;
			manaSlider.value = currentMana;
		}
		else
			Debug.LogWarning("manaSlider is not assigned on " + gameObject.name);
	}

	private void RegenerateMana()
	{
		if (currentMana < maxMana)
		{
			currentMana += manaRegenerationRate * Time.deltaTime;
			if (currentMana > maxMana)
			{
				currentMana = maxMana;
			}
		}
	}

	protected bool HasEnoughMana(int amount) => currentMana >= amount;

	protected void SpendMana(int amount)
	{
		currentMana -= amount;
		currentMana = Mathf.Clamp(currentMana, 0, maxMana);

		if (currentMana < 0)
		{
			currentMana = 0;
		}
	}

	// Status Effects
	public void ApplyBurn(int totalBurnDamage, float burnDuration)
	{
		statusEffectManager?.ApplyBurn(totalBurnDamage, burnDuration);
	}

	public void ApplyStun(float stunDuration)
	{
		statusEffectManager?.ApplyStun(stunDuration);
	}

	// Animations
	protected void PlayAttackAnimation()
	{
		if (!IsOwner) return;
		animator.SetTrigger("AttackTrigger");
	}

	private void PlayDamageAnimation()
	{
		if (!IsOwner) return;
		animator.SetTrigger("DamageTrigger");
	}

	private void PlayDeathAnimation()
	{
		animator.SetTrigger("DieTrigger");
	}

	private void OnDrawGizmosSelected()
	{
		if (attackPoint == null) return;

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attackPoint.position, meleeRange);
	}

	private void OnValidate()
	{
		if (healthSlider != null)
		{
			UpdateHealthSlider();
		}

		if (sliderUI != null && currentHealth != maxHealth)
		{
			sliderUI.FlashHealthOnDamage();
		}
	}

	/// <summary>
	/// Locally calls GameManager.FinalizeMatch and saves to disk.
	/// </summary>
	private void FinalizeLocalMatch(bool won, int kills, int deaths)
	{
		Debug.Log("FinalizeLocalMatch in FighterBase is triggerd");
		var gm = FindObjectOfType<GameManager>();
		if (gm == null)
			Debug.Log("gm is null");
		if (gm != null)
			gm.FinalizeMatch(won, kills, deaths);
	}

	/// <summary>
	/// Client→Server: dying client asks host to notify the opponent(s).
	/// </summary>
	[ServerRpc]
	private void SubmitOpponentWonServerRpc(ServerRpcParams rpcParams = default)
	{
		Debug.Log("SubmitOpponentWonServerRpc has been called, will now call NotifyOpponentWonClientRpc()");
		NotifyOpponentWonClientRpc();
	}

	/// <summary>
	/// Server→Clients: instruct surviving clients to record a win.
	/// </summary>
	[ClientRpc]
	private void NotifyOpponentWonClientRpc(ClientRpcParams rpcParams = default)
	{
		Debug.Log("NotifyOpponentWonClientRpc has been called, will now call FinalizeLocalMatch()");
		
		// Only the surviving player’s owner should run this
		if (IsOwner) return;

		bool iWon = true;
		int myKills = sessionKills;
		int myDeaths = sessionDeaths;
		FinalizeLocalMatch(iWon, myKills, myDeaths);
	}
	
	public override void OnNetworkSpawn()
	{
		base.OnNetworkSpawn();
		if (IsOwner) StartCoroutine(BindHudSlidersWhenReady());
	}

	private IEnumerator BindHudSlidersWhenReady()
	{
		// Wait until HUDManager exists & exposes sliders
		yield return new WaitUntil(() => HUDManager.Instance && HUDManager.Instance.HealthSlider && HUDManager.Instance.ManaSlider);

		healthSlider = HUDManager.Instance.HealthSlider;
		manaSlider   = HUDManager.Instance.ManaSlider;

		UpdateHealthSlider();
		UpdateManaSlider();
	}
	
}