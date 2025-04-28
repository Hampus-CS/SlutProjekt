using TMPro;
using UnityEngine;

public class Wizzard : FighterBase
{
    [Header("Wizzard Settings")]
    public GameObject lightningBoltPrefab;
    public Transform firePoint;
    [Header("Lightning Bolt")]
    public float lightningBoltSpeed = 15f;
    public float lightningBoltCooldown = 3f;
    private float lastLightningBoltTime = -Mathf.Infinity;
   
    [Header("Mana System")]
    private float maxMana = 100f;
    private float currentMana;
    public int lightningBoltCost = 30;
    public float manaRegenerationRate = 5f;

    public TextMeshProUGUI manaText; // temp for testing, preferably a UI slider or something
    private void Start()
    {
        currentMana = maxMana;
    }
    private void Update()
    {
        // Recharge mana slowly over time
        if (currentMana < maxMana)
        {
            currentMana += manaRegenerationRate * Time.deltaTime;
            if (currentMana > maxMana) currentMana = maxMana;
        }
        // Lightning bolt cooldown and mana check
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= lastLightningBoltTime + lightningBoltCooldown)
        {
            if (currentMana >= lightningBoltCost)
            {
                CastLightningBolt();
                currentMana -= lightningBoltCost;
                lastLightningBoltTime = Time.time;
            }
            else
            {
                Debug.Log("Not enough mana!");
            }
        }
        if (manaText != null)
        {
            manaText.text = "Mana: " + Mathf.FloorToInt(currentMana).ToString();  // Display current mana
        }
    }
    void CastLightningBolt()
    {
        GameObject lightningBolt = Instantiate(lightningBoltPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = lightningBolt.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * lightningBoltSpeed;
        Destroy(lightningBolt, 2f); // Destroy after 2 seconds
    }

    public override void Attack(FighterBase opponent)
    {
        int damage = baseAttackPower + 2; // Less than Warrior but maybe could be a ranged attack later or something like that. But for now 
        Debug.Log($"{fighterName} (Mage) casts a fireball at {opponent.fighterName}, dealing {damage} magic damage!");
        opponent.TakeDamage(damage);
    }
}
