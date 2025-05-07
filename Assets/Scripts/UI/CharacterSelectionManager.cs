using UnityEngine;
using System.Collections.Generic;

public class CharacterSelectionManager : MonoBehaviour
{
    [Header("Data")]
    public List<CharacterData> allCharacters;

    [Header("Panel Roots")]
    public GameObject carouselPanelRoot;
    public GameObject detailPanelRoot;

    [Header("Selectors & Display")]
    public CharacterCarouselSelector carouselSelector;
    public CharacterDetailDisplay    detailDisplay;

    private CharacterData selectedCharacter;
    private int selectedAspectIndex;
    
    void Start()
    {
        // 1) Initialize the fighter carousel
        carouselSelector.Initialize(allCharacters);

        // 2) When they confirm a character, show the detail panel
        carouselSelector.OnCharacterChosen += idx =>
        {
            selectedCharacter = allCharacters[idx];
            detailDisplay.SetCharacter(selectedCharacter);

            // Subscribe to the detail‑panel events:
            detailDisplay.OnBack    += ShowCarousel;
            detailDisplay.OnConfirm += OnDetailConfirmed;

            // Fade to detail
            PanelTransition.SwapPanels(carouselPanelRoot, detailPanelRoot, 0.4f);
        };
    }
    /// <summary>
    /// Called when “Back” is pressed in detail. Returns to fighter carousel.
    /// </summary>
    private void ShowCarousel()
    {
        // Unsubscribe detail callbacks to avoid duplicates
        detailDisplay.OnBack    -= ShowCarousel;
        detailDisplay.OnConfirm -= OnDetailConfirmed;

        PanelTransition.SwapPanels(detailPanelRoot, carouselPanelRoot, 0.4f);
    }

    /// <summary>
    /// Called when “Confirm” is pressed in detail. Starts the fight.
    /// </summary>
    private void OnDetailConfirmed(int aspectIdx)
    {
        selectedAspectIndex = aspectIdx;
        LaunchFight();
    }

    /// <summary>
    /// Load the arena scene and spawn the fighter prefab.
    /// </summary>
    private void LaunchFight()
    {
        Debug.Log($"Launching fight as {selectedCharacter.displayName}, aspect {selectedAspectIndex}");
        
        // ex:
        // SceneManager.LoadScene("FightScene");
        // Then in that scene, you could instantiate:
        // Instantiate(selectedCharacter.fighterPrefab, spawnPoint, Quaternion.identity);
    }
}
