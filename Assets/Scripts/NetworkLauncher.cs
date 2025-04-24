using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Simple network launcher for testing multiplayer host/client + join/leave logging.
/// </summary>
public class NetworkLauncher : MonoBehaviour
{
    private void OnGUI()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            GUILayout.BeginArea(new Rect(10, 10, 200, 100), "Start Network", GUI.skin.window);
            if (GUILayout.Button("Start Host"))
            {
                NetworkManager.Singleton.StartHost();
            }
            if (GUILayout.Button("Start Client"))
            {
                NetworkManager.Singleton.StartClient();
            }
            GUILayout.EndArea();
        }
    }

    private void OnEnable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
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
