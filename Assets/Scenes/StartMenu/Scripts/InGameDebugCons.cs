using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class InGameDebugCons : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject consolePanel;
    [SerializeField] private TextMeshProUGUI consoleText;
    [SerializeField] private KeyCode toggleKey = KeyCode.F10;

    private readonly List<string> logLines = new List<string>();
    private const int maxLogLines = 100;

    private void Awake()
    {
        Application.logMessageReceived += HandleLog;
        if (consolePanel != null)
            consolePanel.SetActive(false); // Hide console at start
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (consolePanel != null)
                consolePanel.SetActive(!consolePanel.activeSelf);
        }
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string color = type switch
        {
            LogType.Error => "<color=red>",
            LogType.Warning => "<color=yellow>",
            LogType.Log => "<color=white>",
            _ => "<color=grey>"
        };

        string logEntry = $"{color}{logString}</color>";

        if (type == LogType.Exception)
            logEntry += $"\n{stackTrace}";

        logLines.Add(logEntry);

        if (logLines.Count > maxLogLines)
            logLines.RemoveAt(0);

        UpdateConsoleText();
    }

    private void UpdateConsoleText()
    {
        if (consoleText != null)
            consoleText.text = string.Join("\n", logLines);
    }
}