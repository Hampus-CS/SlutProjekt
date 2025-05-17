using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

/// <summary>
/// Host checks when all players are ready, then loads the FightScene.
/// </summary>
public class SceneReadyChecker : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string fightSceneName = "FightScene";

    private void Update()
    {
        // Only Host should control scene switching
        if (!NetworkManager.Singleton.IsHost)
            return;

        if (AllPlayersReady())
        {
            Debug.Log("[SceneReadyChecker] All players ready! Loading FightScene...");
            NetworkManager.Singleton.SceneManager.LoadScene(fightSceneName, LoadSceneMode.Single);
            enabled = false; // Disable this checker after loading
        }
    }

    private bool AllPlayersReady()
    {
        foreach (var clientPair in NetworkManager.Singleton.ConnectedClientsList)
        {
            var playerObject = clientPair.PlayerObject;
            if (playerObject == null)
                return false;

            var playerData = playerObject.GetComponent<PlayerSelectData>();
            if (playerData == null)
                return false;

            if (!playerData.IsReady.Value)
                return false; // Player not ready yet
        }
        return true; // All players ready
    }
}
