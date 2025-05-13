using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine.SceneManagement;

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
    private PlayerSelectData localPlayerSelectData;

    private void Awake()
    {
        /* Require an active NGO session */
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("[CharacterSelectionManager] No NetworkManager – start Host/Client before loading CharacterSelect.");
            enabled = false;
            return;
        }

        if (NetworkManager.Singleton.LocalClient == null ||
            NetworkManager.Singleton.LocalClient.PlayerObject == null)
        {
            Debug.LogError("[CharacterSelectionManager] Local PlayerObject not spawned yet.");
            enabled = false;
            return;
        }

        localPlayerSelectData = NetworkManager.Singleton.LocalClient.PlayerObject
            .GetComponent<PlayerSelectData>();          // :contentReference[oaicite:0]{index=0}:contentReference[oaicite:1]{index=1}
    }

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
        
        /* Tell the server what we picked */
        localPlayerSelectData.SelectCharacter(selectedCharacter.id);
        localPlayerSelectData.SetReady();

        /* Host: check if everyone is Ready */
        if (NetworkManager.Singleton.IsHost)
            TryStartMatchIfEveryoneReady();
        
    }

    /// <summary>
    /// Host‑only: verify readiness across all clients and load the fight scene.
    /// </summary>
    private void TryStartMatchIfEveryoneReady()
    {
        bool allReady = NetworkManager.Singleton.ConnectedClientsList
            .All(c => c.PlayerObject.GetComponent<PlayerSelectData>().IsReady.Value);

        if (allReady)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                "FightScene", LoadSceneMode.Single);
        }
    }
}
