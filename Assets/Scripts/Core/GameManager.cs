using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SaveManager saveManager;
    public FighterBase player1;
    public FighterBase player2;

    void Start()
    {
        LoadPlayerScores();
    }
    public void EndMatch(FighterBase winner, FighterBase loser) // WIP
    {
        /*
        Debug.Log($"{winner.fighterName} wins!");
        int newScore = saveManager.LoadPlayerScore(winner.fighterName) + 1;
        saveManager.SavePlayerScore(winner.fighterName, newScore);
        */
    }

    void LoadPlayerScores() // WIP
    {
        /*
        Debug.Log($"P1: {saveManager.LoadPlayerScore("Player1")}, P2: {saveManager.LoadPlayerScore("Player2")}");
        */
    }

}