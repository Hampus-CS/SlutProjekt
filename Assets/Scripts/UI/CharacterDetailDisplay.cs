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
    
    public void SetCharacter(CharacterData data)
    {
        currentData = data;

        //nameText.text  = data.displayName;
        //titleText.text = data.title;

        detailCarousel.Initialize(
            data.idleSprite,
            data.basicAttackSprite,
            data.abilitySprite
        );

        detailCarousel.OnDetailIndexChanged += OnAspectChanged;

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(() => OnBack?.Invoke());

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => OnConfirm?.Invoke(currentAspectIndex));
    }

    void OnDisable()
    {
        detailCarousel.OnDetailIndexChanged -= OnAspectChanged;
    }
    
    public event Action OnBack;
    public event Action<int> OnConfirm;

    private void OnAspectChanged(int idx)
    {
        currentAspectIndex = idx;
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