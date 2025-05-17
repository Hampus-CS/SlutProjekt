using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

public class SelectionNetworkController : NetworkBehaviour
{
    public static SelectionNetworkController Instance { get; private set; }

    private Dictionary<ulong, int> selectionMap = new Dictionary<ulong,int>();
    
    // server‐side record
    private HashSet<ulong> readyClients = new HashSet<ulong>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool TryGetSelection(ulong clientId, out int characterId)
    {
        return selectionMap.TryGetValue(clientId, out characterId);
    }

    
    /// <summary>
    /// Called by any client when they confirm their selection.
    /// </summary>
    public void SubmitSelection(int characterId, int aspectIdx)
    {
        SubmitSelectionServerRpc(NetworkManager.Singleton.LocalClientId, characterId, aspectIdx);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SubmitSelectionServerRpc(ulong clientId, int characterId, int aspectIdx)
    {
        selectionMap[clientId] = characterId;
        
        readyClients.Add(clientId);
        Debug.Log($"[Server] Client {clientId} ready (char {characterId}, asp {aspectIdx})");

        int total = NetworkManager.Singleton.ConnectedClientsList.Count;
        if (readyClients.Count >= total)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("FightScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
