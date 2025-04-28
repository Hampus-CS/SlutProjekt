using UnityEngine;

public class LightningBolt : MonoBehaviour
{
    public int impactDamage = 10;
    public float stunDuration = 2f;

    private void Start()
    {
        Destroy(gameObject, 10f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        FighterBase target = other.GetComponent<FighterBase>();
        if (target != null)
        {
            target.TakeDamage(impactDamage);
            target.Stun(stunDuration);
            Destroy(gameObject);
        }
    }
}
