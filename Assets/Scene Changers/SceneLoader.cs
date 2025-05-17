using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

/// <summary>
/// Monitors player connections and transitions to the next scene when conditions are met.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string characterSelectSceneName = "CharacterSelect";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        // Only host should manage scene transitions
        if (!NetworkManager.Singleton.IsHost)
            return;

        int connectedClients = NetworkManager.Singleton.ConnectedClientsList.Count;

        Debug.Log($"[SceneLoader] Connected clients: {connectedClients}");

        // When 2 players are connected (host + client), load CharacterSelect
        if (connectedClients == 2)
        {
            Debug.Log("[SceneLoader] Both players connected. Loading CharacterSelect scene...");

            NetworkManager.Singleton.SceneManager.LoadScene(characterSelectSceneName, LoadSceneMode.Single);
        }
    }
}
