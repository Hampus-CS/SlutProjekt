using UnityEngine;
using Unity.Netcode;

public class SelectionNetworkBootstrap : MonoBehaviour
{
    [Tooltip("Drag your SelectionNetworkController prefab (as a GameObject) here")]
    [SerializeField] private GameObject selectionControllerPrefabGO;

    private void Awake()
    {
        if (!NetworkManager.Singleton.IsServer) return;
        if (SelectionNetworkController.Instance != null) return;

        // Instantiate the prefab GameObject
        var go = Instantiate(selectionControllerPrefabGO);
        // Grab its NetworkObject to spawn it
        var netObj = go.GetComponent<NetworkObject>();
        netObj.Spawn();

        Debug.Log("[Bootstrap] Spawned SelectionNetworkController.");
    }
}
