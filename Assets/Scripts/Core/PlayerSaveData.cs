using UnityEngine;

/// <summary>
/// Stores all match statistics for a player. Uses encapsulation to protect data access.
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
    
    // Public read-only access
    public int TotalKills     => totalKills;
    public int TotalDeaths    => totalDeaths;
    public int TotalWins      => totalWins;
    public int TotalLosses    => totalLosses;
    public int MatchesPlayed  => matchesPlayed;
    
    /// <summary>Called at the end of a match to update statistics.</summary>
    public void RecordMatch(int kills, int deaths, bool won)
    {
        totalKills    += kills;
        totalDeaths   += deaths;
        matchesPlayed += 1;

        if (won)
            totalWins++;
        else
            totalLosses++;
    }

    /// <summary>Resets all stats to zero.</summary>
    public void ResetAll()
    {
        totalKills = totalDeaths = totalWins = totalLosses = matchesPlayed = 0;
    }

    /// <summary>Returns a short summary of current stats.</summary>
    public override string ToString()
    {
        return $"Matches: {matchesPlayed} | Wins: {totalWins} | Losses: {totalLosses} | Kills: {totalKills} | Deaths: {totalDeaths}";
    }
}