using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

/// <summary>
/// Spawns the correct selected character prefab for each player.
/// </summary>
public class FightSpawner : MonoBehaviour
{
    [Header("Character Prefabs")]
    [SerializeField] private List<GameObject> characterPrefabs; // List ordered by ID

    [Header("Spawn Settings")]
    [SerializeField] private Transform[] spawnPoints; // Points where players spawn

    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            SpawnPlayers();
        }
    }

    private void SpawnPlayers()
    {
        int spawnIndex = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = client.PlayerObject;
            if (playerObject == null)
                continue;

            var playerData = playerObject.GetComponent<PlayerSelectData>();
            if (playerData == null)
                continue;

            int selectedCharacterId = playerData.SelectedCharacterId.Value;

            if (selectedCharacterId >= 0 && selectedCharacterId < characterPrefabs.Count)
            {
                GameObject prefab = characterPrefabs[selectedCharacterId];
                Vector3 spawnPosition = spawnPoints[Mathf.Min(spawnIndex, spawnPoints.Length - 1)].position;
                Quaternion spawnRotation = Quaternion.identity;

                GameObject playerInstance = Instantiate(prefab, spawnPosition, spawnRotation);
                playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(client.ClientId);

                Debug.Log($"[FightSpawner] Spawned player {client.ClientId} with character ID {selectedCharacterId} at {spawnPosition}");
            }
            else
            {
                Debug.LogWarning($"[FightSpawner] Invalid character ID for player {client.ClientId}");
            }

            spawnIndex++;
        }
    }
}
