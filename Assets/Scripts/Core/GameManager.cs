using Unity.Netcode;
using UnityEngine;

/// <summary>
/// GameManager handles match logic, score management,
/// and enforces host-only write access for saving.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        // To ensure one GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Called by server when the match ends.  Pass the winning player's ClientId.
    /// </summary>
    public void EndMatchServer(ulong winnerClientId)
    {
        if (!IsServer) return;

        // Broadcast to each client whether they won
        EndMatchClientRpc(winnerClientId);
    }

    [ClientRpc]
    private void EndMatchClientRpc(ulong winnerClientId, ClientRpcParams rpcParams = default)
    {
        bool didIWin = NetworkManager.Singleton.LocalClientId == winnerClientId;

        // Grab local session stats from FighterBase
        int kills  = FighterBase.sessionKills;
        int deaths = FighterBase.sessionDeaths;

        // Record them locally
        SaveManager.Instance.SaveMatchResult(kills, deaths, didIWin);

        Debug.Log($"[GameManager] Client {NetworkManager.Singleton.LocalClientId} " +
                  $"Match Over (Win? {didIWin}) -- Kills: {kills}, Deaths: {deaths}");

        // Reset for next match
        FighterBase.sessionKills  = 0;
        FighterBase.sessionDeaths = 0;

        // Optionally load all-time stats and display
        var allStats = SaveManager.Instance.LoadStats();
        Debug.Log($"[All-Time Stats] Matches: {allStats.matchesPlayed}, " +
                  $"Wins: {allStats.totalWins}, Losses: {allStats.totalLosses}, " +
                  $"Kills: {allStats.totalKills}, Deaths: {allStats.totalDeaths}");

        // TODO: transition back to lobby or show rematch UI here…
    }
    /// <summary>
    /// Call this on the host when you detect the match winner (e.g. via health checks).
    /// </summary>
    public void DeclareWinner(ulong winningClientId)
    {
        if (IsServer)
            EndMatchServer(winningClientId);
    }

    /// <summary>
    /// Reset fighters' health, session stats, etc., to start a new match.
    /// </summary>
    public void StartMatch()
    {
        FighterBase.sessionKills  = 0;
        FighterBase.sessionDeaths = 0;

        // References to the fighters, heal them:
        player1.currentHealth = player1.maxHealth;
        player2.currentHealth = player2.maxHealth;
    }
}