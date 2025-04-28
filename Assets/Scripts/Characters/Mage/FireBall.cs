using UnityEngine;

public class Fireball : MonoBehaviour
{
    private int impactDamage = 10;
    private int totalBurnDamage = 10;
    private float burnDuration = 5f;

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
            target.ApplyBurn(totalBurnDamage, burnDuration);
            Destroy(gameObject);
        }
    }
}
