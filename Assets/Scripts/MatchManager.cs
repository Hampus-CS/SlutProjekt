using UnityEngine;
using Unity.Netcode;

public class MatchManager : MonoBehaviour
{
    // Call this when the fight is definitively over on the local client
    // 'didIWin' indicates whether this player won
    public void EndMatch(bool didIWin)
    {
        // Retrieve session tallies from FighterBase
        int kills  = FighterBase.sessionKills;
        int deaths = FighterBase.sessionDeaths;

        // Save these results locally
        SaveManager.Instance.SaveMatchResult(kills, deaths, didIWin);

        Debug.Log($"[MatchManager] Client {NetworkManager.Singleton.LocalClientId} results - " +
                  $"Kills: {kills}, Deaths: {deaths}, Win: {didIWin}");

        // Optionally, reset session counters for next match
        FighterBase.sessionKills  = 0;
        FighterBase.sessionDeaths = 0;

        // TODO: Transition back to lobby / show stats
    }
}
