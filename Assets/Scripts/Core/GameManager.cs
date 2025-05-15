using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Responsible for saving local match result for the current player after the fight ends.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Match Outcome")]
    [SerializeField] private bool playerWon = false;
    [SerializeField] private int killsThisMatch = 0;
    [SerializeField] private int deathsThisMatch = 0;

    private bool hasSaved = false;

    /// <summary>
    /// Call this at the end of the match when the player dies or wins.
    /// </summary>
    public void FinalizeMatch(bool won, int kills, int deaths)
    {
        if (hasSaved)
            return; // prevent double-saving

        playerWon = won;
        killsThisMatch = kills;
        deathsThisMatch = deaths;

        var stats = SaveManager.LoadPlayerData();
        stats.RecordMatch(killsThisMatch, deathsThisMatch, playerWon);
        SaveManager.SavePlayerData(stats);

        Debug.Log("[GameManager] Match stats saved:");
        Debug.Log(stats.ToString());

        hasSaved = true;
    }

    /// <summary>
    /// Reset the saved state, if rematching or returning to menu.
    /// </summary>
    public void ResetMatchData()
    {
        hasSaved = false;
        killsThisMatch = 0;
        deathsThisMatch = 0;
        playerWon = false;
    }
}