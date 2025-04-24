using System.Collections.Generic;

/// <summary>
/// Serializable save data structure to track player scores.
/// Can be expanded to include more persistent stats (e.g., kills, settings, etc.)
/// </summary>
[System.Serializable]
public class PlayerSaveData
{
    public List<PlayerEntry> scores = new();

    public void SetScore(string playerID, int score)
    {
        var existing = scores.Find(e => e.playerID == playerID);
        if (existing != null)
        {
            existing.score = score;
        }
        else
        {
            scores.Add(new PlayerEntry { playerID = playerID, score = score });
        }
    }

    public int GetScore(string playerID)
    {
        var existing = scores.Find(e => e.playerID == playerID);
        return existing != null ? existing.score : 0;
    }
}

[System.Serializable]
public class PlayerEntry
{
    public string playerID;
    public int score;
}