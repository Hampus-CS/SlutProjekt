using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Spawns the correct selected character prefab for each player.
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
        int spawnIndex = 0;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerSelectData = client.PlayerObject.GetComponent<PlayerSelectData>();
            int id    = playerSelectData.SelectedCharacterId.Value;
            var data  = allCharacters.FirstOrDefault(c => c.id == id);

            if (data == null)
            {
                Debug.LogWarning($"Unknown character id {id}");
                continue;
            }

            var go = Instantiate(data.fighterPrefab,
                spawnPoints[spawnIndex].position,
                Quaternion.identity);

            go.GetComponent<NetworkObject>()
                .SpawnAsPlayerObject(client.ClientId);

            spawnIndex++;
        }
    }
}
