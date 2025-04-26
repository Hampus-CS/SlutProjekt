using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

/// <summary>
/// Manages the Character Select screen UI.
/// </summary>
public class CharacterSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button[] characterButtons; // One button per character
    [SerializeField] private Button readyButton;
    [SerializeField] private TextMeshProUGUI selectedCharacterText;

    private PlayerSelectData playerSelectData;

    private void Start()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.LocalClient != null)
        {
            var localPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            if (localPlayerObject != null)
            {
                playerSelectData = localPlayerObject.GetComponent<PlayerSelectData>();
            }
        }

        foreach (var button in characterButtons)
        {
            button.onClick.AddListener(() => OnCharacterButtonClicked(button));
        }

        if (readyButton != null)
        {
            readyButton.onClick.AddListener(OnReadyButtonClicked);
        }
    }

    private void OnCharacterButtonClicked(Button button)
    {
        if (playerSelectData != null)
        {
            int characterId = button.transform.GetSiblingIndex(); // Assume button index = character ID
            playerSelectData.SelectCharacter(characterId);

            if (selectedCharacterText != null)
            {
                selectedCharacterText.text = $"Selected Character: {button.name}";
            }
        }
    }

    private void OnReadyButtonClicked()
    {
        if (playerSelectData != null)
        {
            playerSelectData.SetReady();
            readyButton.interactable = false;
        }
    }
}
