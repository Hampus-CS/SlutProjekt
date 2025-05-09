using UnityEngine;

public class MagicBolt : MonoBehaviour
{
    private GameObject owner;
    
    public int damage;
    
    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    public void SetOwner(GameObject caster)
    {
        owner = caster;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return;
        
        FighterBase target = other.GetComponent<FighterBase>();
        if (target != null)
        {
            target.TakeDamage(Mage.dama);
        }        
    }
}
