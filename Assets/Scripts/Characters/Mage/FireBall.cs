using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour
{
    public int impactDamage = 10;
    public int totalBurnDamage = 10;
    public float burnDuration = 5f;
    public float burnTickInterval = 1f;

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

            target.ApplyBurn(totalBurnDamage, burnDuration, burnTickInterval);

            Destroy(gameObject);
        }
    }
}