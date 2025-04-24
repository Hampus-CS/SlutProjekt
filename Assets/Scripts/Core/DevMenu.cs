#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// In-editor-only developer debug menu. Toggle with T.
/// </summary>
public class DevMenu : MonoBehaviour
{
    public FighterBase player1;
    public FighterBase player2;

    private bool showMenu = false;
    private bool showStats = true;

    private readonly Queue<string> damageLog = new();
    private float dpsTimer = 0f;
    private float dpsWindow = 3f; // Rolling window in seconds
    private int recentDamage = 0;

    void Start()
    {
        if (player1 != null) player1.OnDamaged += (dmg) => LogDamage(player1.fighterName, dmg);
        if (player2 != null) player2.OnDamaged += (dmg) => LogDamage(player2.fighterName, dmg);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            showMenu = !showMenu;
        }

        // Decay DPS counter
        dpsTimer -= Time.deltaTime;
        if (dpsTimer < 0f)
        {
            recentDamage = 0;
            dpsTimer = 0f;
        }
    }

    void OnGUI()
    {
        if (!showMenu) return;

        GUILayout.BeginArea(new Rect(10, 10, 320, 300), "DEV MENU", GUI.skin.window);

        GUILayout.Label("🧪 Debug Controls");

        if (GUILayout.Button("Heal Player 1")) player1.currentHealth = player1.maxHealth;
        if (GUILayout.Button("Heal Player 2")) player2.currentHealth = player2.maxHealth;
        if (GUILayout.Button("Kill Player 2")) player2.TakeDamage(9999);
        if (GUILayout.Button("Log Player Scores"))
        {
            int p1Score = SaveManager.Instance.LoadPlayerScore(player1.fighterName);
            int p2Score = SaveManager.Instance.LoadPlayerScore(player2.fighterName);

            Debug.Log($"{player1.fighterName} Score: {p1Score}");
            Debug.Log($"{player2.fighterName} Score: {p2Score}");
        }

        GUILayout.Space(10);
        showStats = GUILayout.Toggle(showStats, "📊 Show Real-Time Stats");

        if (showStats)
        {
            GUILayout.Label($"🔥 Current DPS: {recentDamage / dpsWindow:F2} (rolling {dpsWindow}s)");
            GUILayout.Label("🩸 Damage Log:");

            foreach (var log in damageLog)
            {
                GUILayout.Label(log);
            }
        }

        GUILayout.EndArea();
    }

    private void LogDamage(string playerName, int damage)
    {
        string entry = $"{Time.time:F1}s: {playerName} took {damage}";
        damageLog.Enqueue(entry);
        recentDamage += damage;
        dpsTimer = dpsWindow;

        while (damageLog.Count > 5)
            damageLog.Dequeue();
    }
}
#endif