using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CharacterDetailDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterDetailCarouselSelector detailCarousel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Button backButton;
    [SerializeField] private Button confirmButton;

    private CharacterData currentData;
    private int currentAspectIndex = 0;
    
    // Event fired by this display when user wants to go back
    public event Action OnBack;
    // Event fired when user confirms; passes chosen aspect index (0=Idle,1=Basic,2=Ability)
    public event Action<int> OnConfirm;
    
    /// <summary>
    /// Called by the manager when entering the detail panel.
    /// </summary>
    public void SetCharacter(CharacterData data)
    {
        currentData = data;
        //nameText.text  = data.displayName;
        //titleText.text = data.title;

        // Initialize the carousel with animations & fallbacks
        detailCarousel.Initialize(data.uiIdleClip, data.fallbackIdleSprite, data.uiBasicAttackClip, data.fallbackBasicAttackSprite, data.uiAbilityClip, data.fallbackAbilitySprite);

        // Subscribe to index changes
        detailCarousel.OnDetailIndexChanged += OnAspectChanged;

        // Wire buttons
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => OnBack?.Invoke());

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => OnConfirm?.Invoke(currentAspectIndex));
    }

    void OnDisable()
    {
        // Unsubscribe when panel is hidden to avoid leaks
        detailCarousel.OnDetailIndexChanged -= OnAspectChanged;
    }
    
    private void OnAspectChanged(int idx)
    {
        currentAspectIndex = idx;

        // Update the description text based on aspect
        switch (idx)
        {
            case 0:
                descriptionText.text = currentData.lore;
                break;
            case 1:
                descriptionText.text = currentData.basicAttackDescription;
                break;
            case 2:
                descriptionText.text = currentData.abilityDescription;
                break;
        }
    }
}