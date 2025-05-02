using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character Data", fileName = "NewCharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    public int id; // 0,1,2 : MUST be unique
    public string displayName;
    [TextArea] public string lore;
    [TextArea] public string abilityDescription;
    [TextArea] public string basicAttackDescription;

    [Header("Artwork")]
    public Sprite idleSprite; // Carousel + main portrait
    public Sprite basicAttackSprite;
    public Sprite abilitySprite;

    [Header("Gameplay")]
    public GameObject fighterPrefab; // Prefab to spawn in the arena
    public int maxHealth = 100; // For HUD preview, not used by code yet
    public int attackPower = 10;
}
