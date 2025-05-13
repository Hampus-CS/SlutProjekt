using UnityEngine;

public class MagicBolt : MonoBehaviour
{
    public int damage = 10;

    private void Start()
    {
        Destroy(gameObject, 2.5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == this.gameObject) return;

        FighterBase target = other.GetComponent<FighterBase>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
        
        Destroy(gameObject);
        
    }
}