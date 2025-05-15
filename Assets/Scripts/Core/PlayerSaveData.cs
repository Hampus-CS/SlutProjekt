using UnityEngine;

/// <summary>
/// Serializable save data structure to track player scores.
/// Can be expanded to include more persistent stats (e.g., kills, settings, etc.)
/// </summary>
[System.Serializable]
public class PlayerSaveData
{
    
    [Header("Stats")]
    [SerializeField] private int totalKills      = 0;
    [SerializeField] private int totalDeaths     = 0;
    [SerializeField] private int totalWins       = 0;
    [SerializeField] private int totalLosses     = 0;
    [SerializeField] private int matchesPlayed   = 0;
    
    /// <summary>
    /// Call once per match to update cumulative stats.
    /// </summary>
    /// <param name="kills">Kills in this match</param>
    /// <param name="deaths">Deaths in this match</param>
    /// <param name="won">True if this match was won</param>
    public void RegisterMatch(int kills, int deaths, bool won)
    {
        matchesPlayed++;
        totalKills  += kills;
        totalDeaths += deaths;
        if (won) totalWins++;
        else totalLosses++;
    }
}