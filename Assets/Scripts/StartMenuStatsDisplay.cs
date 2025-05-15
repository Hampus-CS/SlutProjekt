using UnityEngine;
using TMPro;

/// <summary>
/// Loads and displays the current player's saved stats when the StartMenu scene loads.
/// </summary>
public class StartMenuStatsDisplay : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI statsText;

    private void Start()
    {
        DisplayPlayerStats();
    }

    private void DisplayPlayerStats()
    {
        if (statsText == null)
        {
            Debug.LogWarning("[StartMenuStatsDisplay] No statsText assigned.");
            return;
        }

        var stats = SaveManager.LoadPlayerData();

        statsText.text = $"<b>Welcome Back!</b>\n\n" + $"<b>Matches Played:</b> {stats.MatchesPlayed}\n" + $"<b>Wins:</b> {stats.TotalWins} | <b>Losses:</b> {stats.TotalLosses}\n" + $"<b>Kills:</b> {stats.TotalKills} | <b>Deaths:</b> {stats.TotalDeaths}";
    }
}
