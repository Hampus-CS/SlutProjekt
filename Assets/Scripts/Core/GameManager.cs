using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Responsible for saving local match result for the current player after the fight ends.
/// </summary>
public class GameManager : NetworkBehaviour
{
    [Header("Match Outcome")]
    [SerializeField] private bool playerWon = false;
    [SerializeField] private int killsThisMatch = 0;
    [SerializeField] private int deathsThisMatch = 0;
    
    private bool matchEnded = false;
    private bool hasSaved = false;

    private void Awake()
    {
	    Debug.Log($"[GameManager] IsServer = {IsServer}, IsClient = {IsClient}, IsOwner = {IsOwner}");
    }
    
    /// <summary>
    /// Call this at the end of the match when the player dies or wins.
    /// </summary>
    public void FinalizeMatch(bool won, int kills, int deaths)
    {
	    if (matchEnded) return;
	    matchEnded = true;

	    var stats = SaveManager.LoadPlayerData();
	    stats.RecordMatch(kills, deaths, won);
	    SaveManager.SavePlayerData(stats);

	    Debug.Log("[GameManager] Match stats saved:");
	    Debug.Log(stats.ToString());

	    StartCoroutine(EndMatchAfterDelay(2f));
    }

    private IEnumerator EndMatchAfterDelay(float delay)
    {
	    yield return new WaitForSeconds(delay);

	    if (IsServer)
	    {
		    Debug.Log("[GameManager] Ending match. Returning to StartMenu for all players.");
		    NetworkManager.Singleton.SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
	    }
    }
    
}