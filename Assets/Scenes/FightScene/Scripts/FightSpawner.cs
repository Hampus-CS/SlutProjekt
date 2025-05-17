using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Spawns the correct selected character prefab for each player.
/// reading their choice from the SelectionNetworkController’s server‐side map.
/// </summary>
public class FightSpawner : MonoBehaviour
{
    [Header("Character Prefabs")]
    [SerializeField] private List<CharacterData> allCharacters;
    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints; // Points where players spawn

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
            SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        var controller = SelectionNetworkController.Instance;
        if (controller == null)
        {
            Debug.LogError("[FightSpawner] No SelectionNetworkController found!");
            return;
        }
        
        int spawnIndex = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            // Ask the controller for this client’s chosen character ID
            if (!controller.TryGetSelection(client.ClientId, out int charId))
            {
                Debug.LogWarning($"[FightSpawner] No selection recorded for client {client.ClientId}");
                continue;
            }

            // Find the CharacterData for that ID
            var data = allCharacters.FirstOrDefault(c => c.id == charId);
            if (data == null)
            {
                Debug.LogWarning($"[FightSpawner] Unknown character id {charId} for client {client.ClientId}");
                continue;
            }

            // Instantiate and spawn the prefab
            var go = Instantiate(
                data.fighterPrefab,
                spawnPoints[spawnIndex].position,
                Quaternion.identity
            );
            var netObj = go.GetComponent<NetworkObject>();
            if (netObj == null)
            {
                Debug.LogError("[FightSpawner] Prefab missing NetworkObject component!");
                Destroy(go);
                continue;
            }

            netObj.SpawnWithOwnership(client.ClientId);
            spawnIndex++;
            
        }
    }
}
