using UnityEngine;

public class MagicBolt : MonoBehaviour
{
    public int damage = 10;

    public FighterBase attacker;
    
    private void Start()
    {
        Destroy(gameObject, 2.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FighterBase target = other.GetComponent<FighterBase>();
        if (target != null)
        {
            target.TakeDamage(damage, attacker);
        }
        
        Destroy(gameObject);
    }
}