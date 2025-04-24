using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple network launcher for testing multiplayer host/client + join/leave logging.
/// </summary>
public class NetworkLauncher : MonoBehaviour
{
    [Header("Buttons")]
    public Button hostButton;
    public Button clientButton;

    private void Start()
    {
        // Hook up button listeners
        hostButton.onClick.AddListener(StartAsHost);
        clientButton.onClick.AddListener(StartAsClient);

        // Join/leave log callbacks
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        // Unhook listeners to avoid memory leaks
        hostButton.onClick.RemoveListener(StartAsHost);
        clientButton.onClick.RemoveListener(StartAsClient);

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    public void StartAsHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log("[NetworkLauncherUI] Started as Host");
    }

    public void StartAsClient()
    {
        NetworkManager.Singleton.StartClient();
        Debug.Log("[NetworkLauncherUI] Started as Client");
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[Netcode] Player Connected: ClientID = {clientId}");

        // Example: if clientId == NetworkManager.Singleton.LocalClientId => this is me
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[Netcode] Player Disconnected: ClientID = {clientId}");
    }
}
