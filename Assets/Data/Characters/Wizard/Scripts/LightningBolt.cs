using UnityEngine;

public class LightningBolt : MonoBehaviour
{
    public int impactDamage = 10;
    private float stunDuration = 2f;

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
            target.TakeDamage(impactDamage, attacker);
            target.ApplyStun(stunDuration);
            Destroy(gameObject);
        }
    }
}