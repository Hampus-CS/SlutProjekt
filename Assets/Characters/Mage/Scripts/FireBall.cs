using UnityEngine;

public class Fireball : MonoBehaviour
{
	private int impactDamage = 10;
	private int totalBurnDamage = 10;
	private float burnDuration = 5f;

	[SerializeField]
	private Animator animator;

	public FighterBase attacker;

	private void Start()
	{
		animator = GetComponent<Animator>();
		Destroy(gameObject, 2.5f);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		FighterBase target = other.GetComponent<FighterBase>();
		if (target != null)
		{
			if (target == attacker)
				return;
			
			attacker.DealDamageServerRpc(target.NetworkObject, attacker.NetworkObject, impactDamage);
			attacker.ApplyBurnServerRpc(target.NetworkObject, totalBurnDamage, burnDuration);
			Destroy(gameObject);
		}
	}
}