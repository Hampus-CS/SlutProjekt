using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterDetailDisplay : CharacterCarouselSelector
{
    [Header("Image Slots")]
    public Image portraitIdle;
    public Image basicAttack;
    public Image abilityAttack;

    [Header("Text Slots")]
    public TMP_Text nameText;
    public TMP_Text loreText;
    public TMP_Text basicAttackText;
    public TMP_Text abilityText;

    void OnEnable()
    {
        LoadCharacterDetails();
    }

    void LoadCharacterDetails()
    {
        CharacterData selected = CharacterDetailContext.SelectedCharacter;

        if (selected == null)
        {
            Debug.LogError("No character selected!");
            return;
        }

        portraitIdle.sprite = selected.idleSprite;
        basicAttack.sprite = selected.basicAttackSprite;
        abilityAttack.sprite = selected.abilitySprite;

        nameText.text = selected.displayName;
        loreText.text = selected.lore;
        basicAttackText.text = selected.basicAttackDescription;
        abilityText.text = selected.abilityDescription;
    }
}