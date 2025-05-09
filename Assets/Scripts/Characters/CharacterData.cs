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
    
    [Header("UI Animation Clips")]
    public AnimationClip uiIdleClip;
    public AnimationClip uiBasicAttackClip;
    public AnimationClip uiAbilityClip;
    
    [Header("UI Sprites (fallback)")]
    [Tooltip("Static sprite used only if no idle animation is assigned")]
    public Sprite fallbackIdleSprite;
    [Tooltip("Static sprite used only if no basic‑attack animation is assigned")]
    public Sprite fallbackBasicAttackSprite;
    [Tooltip("Static sprite used only if no ability animation is assigned")]
    public Sprite fallbackAbilitySprite;

    [Header("Descriptions")]
    [TextArea] public string lore;
    [TextArea] public string abilityDescription;
    [TextArea] public string basicAttackDescription;
    
    [Header("In‑Game Prefab")]
    [Tooltip("The prefab instantiated when the match starts")]
    public GameObject fighterPrefab;
}
