using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

/// <summary>
/// Starts multiplayer session, loads the game scene,
/// and spawns players at fixed positions (SpawnPointA and SpawnPointB).
/// Only 2 players are allowed.
/// </summary>
public class NetworkLauncher : MonoBehaviour
{
    [Header("Buttons")]
    public Button hostButton;
    public Button clientButton;

    [Header("Scene")]
    public string gameSceneName = "HCS"; // TODO: Change this game scene name at a later date instead of the testing scene!

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
        if (hostButton != null) hostButton.onClick.RemoveListener(StartAsHost);
        if (clientButton != null) clientButton.onClick.RemoveListener(StartAsClient);

        if (NetworkManager.Singleton == null) return;

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
    }

    public void StartAsHost()
    {
        Debug.Log("[NetworkLauncherUI] Starting as Host...");
        // Add fade or something simillar
        StartCoroutine(DelayedHostStart());
    }
    
    private IEnumerator DelayedHostStart()
    {
        yield return null; // Wait a frame to let NetworkManager initialize

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("[NetworkLauncher] NetworkManager.Singleton is null!");
            yield break;
        }

        NetworkManager.Singleton.StartHost();

        // Wait until SceneManager is initialized
        yield return new WaitUntil(() => NetworkManager.Singleton.SceneManager != null);

        Debug.Log("[NetworkLauncher] SceneManager is now available");

        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    public void StartAsClient()
    {
        Debug.Log("[NetworkLauncherUI] Starting as Client...");
        // Add fade or something simillar
        StartCoroutine(DelayedClientStart());
    }

    private IEnumerator DelayedClientStart()
    {
        yield return null; // allow one frame delay

        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("[NetworkLauncher] NetworkManager.Singleton is null on client!");
            yield break;
        }

        NetworkManager.Singleton.StartClient();

        // Wait until the SceneManager is initialized
        yield return new WaitUntil(() => NetworkManager.Singleton.SceneManager != null);

        Debug.Log("[NetworkLauncher] Client SceneManager is ready");

        NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadMode)
    {
        if (sceneName != gameSceneName) return;

        Debug.Log($"[NetworkLauncher] Scene loaded: {sceneName} for Client {clientId}");

        if (IsLocalPlayer(clientId))
        {
            RequestSpawnPlayerServerRpc();
        }

        NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        // Add fade or something simillar
    }

    private bool IsLocalPlayer(ulong clientId)
    {
        return clientId == NetworkManager.Singleton.LocalClientId;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnPlayerServerRpc(ServerRpcParams rpcParams = default)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count > 2)
        {
            Debug.LogWarning("[NetworkLauncher] Cannot spawn more than 2 players.");
            return;
        }

        GameObject playerPrefab = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;

        if (playerPrefab == null)
        {
            Debug.LogError("[NetworkLauncher] Player Prefab is not assigned in NetworkManager!");
            return;
        }

        Transform spawnPoint = GetSpawnPointForClient(rpcParams.Receive.SenderClientId);

        if (spawnPoint == null)
        {
            Debug.LogError("[NetworkLauncher] Could not determine a valid spawn point.");
            return;
        }

        GameObject player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        player.GetComponent<NetworkObject>().SpawnAsPlayerObject(rpcParams.Receive.SenderClientId);

        Debug.Log($"[NetworkLauncher] Spawned player at {spawnPoint.name} for ClientID {rpcParams.Receive.SenderClientId}");
    }

    private Transform GetSpawnPointForClient(ulong clientId)
    {
        int index = NetworkManager.Singleton.ConnectedClientsIds.ToList().IndexOf(clientId);

        string[] spawnNames = { "SpawnPointA", "SpawnPointB" };

        if (index >= 0 && index < spawnNames.Length)
        {
            GameObject spawnObj = GameObject.Find(spawnNames[index]);
            if (spawnObj != null)
                return spawnObj.transform;
        }

        return null;
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[NetworkLauncher] Client connected: {clientId}");

        if (NetworkManager.Singleton.ConnectedClients.Count > 2)
        {
            Debug.LogWarning("[NetworkLauncher] Max player limit reached. Kicking extra client.");
            NetworkManager.Singleton.DisconnectClient(clientId);
        }

        // Only host triggers scene load for all
        if (NetworkManager.Singleton.IsHost && SceneManager.GetActiveScene().name != gameSceneName)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        Debug.Log($"[NetworkLauncher] Client disconnected: {clientId}");
    }
}
