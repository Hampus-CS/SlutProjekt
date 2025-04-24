using Unity.Netcode;
using UnityEngine;

/// <summary>
/// GameManager handles match logic, score management,
/// and enforces host-only write access for saving.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    public SaveManager saveManager;
    public FighterBase player1;
    public FighterBase player2;

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

    void Start()
    {
        if (saveManager == null)
            Debug.LogWarning("[GameManager] SaveManager not assigned!");
        
        LoadPlayerScores();
    }

    /// <summary>
    /// Called when a match ends — updates score and logs results.
    /// Only host/offline players may update scores.
    /// </summary>
    public void EndMatch(FighterBase winner, FighterBase loser) // WIP
    {
        Debug.Log($"[GameManager] {winner.fighterName} wins the match against {loser.fighterName}!");

        if (saveManager != null && IsHostOrOffline())
        {
            try
            {
                int oldScore = saveManager.LoadPlayerScore(winner.fighterName);
                saveManager.SavePlayerScore(winner.fighterName, oldScore + 1);
                Debug.Log($"[GameManager] Updated score for {winner.fighterName} to {oldScore + 1}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[GameManager] Failed to save score: {ex.Message}");
            }
        }

        // TODO: Trigger rematch, show UI, or reload scene
    }

    /// <summary>
    /// Loads and logs scores at the beginning of a match.
    /// </summary>
    void LoadPlayerScores() // WIP
    {
        if (saveManager == null) return;

        try
        {
            int p1Score = saveManager.LoadPlayerScore(player1.fighterName);
            int p2Score = saveManager.LoadPlayerScore(player2.fighterName);
            Debug.Log($"[GameManager] {player1.fighterName} Score: {p1Score} | {player2.fighterName} Score: {p2Score}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[GameManager] Failed to load player scores: {ex.Message}");
        }
    }

    /// <summary>
    /// Utility to check if we are host or running offline (no networking).
    /// </summary>
    private bool IsHostOrOffline()
    {
        if (NetworkManager.Singleton == null)
            return true;

        return NetworkManager.Singleton.IsHost || !NetworkManager.Singleton.IsClient;
    }

    /// <summary>
    /// Call this to manually start a match if needed.
    /// </summary>
    public void StartMatch()
    {
        Debug.Log("[GameManager] Match started!");

        // Optional: Reset fighters, UI, power-ups etc.
        player1.currentHealth = player1.maxHealth;
        player2.currentHealth = player2.maxHealth;
    }

}