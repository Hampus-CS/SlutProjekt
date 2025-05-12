using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/Character Data", fileName = "NewCharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Unique numeric ID for this character. Must not collide with others.")]
    public int id;
    public string displayName;
    public string title;
    
    [Header("UI Sprites")]
    [Tooltip("Static sprite for the idle pose in the carousel slots")]
    public Sprite idleSprite;
    [Tooltip("Static sprite for the basic-attack preview in detail panel")]
    public Sprite basicAttackSprite;
    [Tooltip("Static sprite for the special-ability preview in detail panel")]
    public Sprite abilitySprite;

    [Header("Descriptions")]
    [TextArea] public string lore;
    [TextArea] public string basicAttackDescription;
    [TextArea] public string abilityDescription;
    
    [Header("In-Game Prefab")]
    [Tooltip("The prefab instantiated when the match starts")]
    public GameObject fighterPrefab;
}