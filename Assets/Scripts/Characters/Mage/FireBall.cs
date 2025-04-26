using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour
{
    public int impactDamage = 5;
    public int burnDamage = 5;
    public float burnDuration = 3f;
    public float burnTickInterval = 1f;

    private void Start()
    {
        Destroy(gameObject, burnDuration);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FighterBase target = other.GetComponent<FighterBase>();
        if (target != null)
        {
            target.TakeDamage(impactDamage);

            // Apply burn effect if it's a valid target
            StartCoroutine(ApplyBurnEffect(target));

            Destroy(gameObject);
        }
    }

    // Coroutine for applying burn damage over time
    private IEnumerator ApplyBurnEffect(FighterBase target)
    {
        float burnEndTime = Time.deltaTime + burnDuration;

        while (Time.deltaTime < burnEndTime)
        {
            // Apply burn damage every tick
            target.TakeDamage(burnDamage);

            // Wait for the next tick
            yield return new WaitForSeconds(burnTickInterval);
        }
    }
}