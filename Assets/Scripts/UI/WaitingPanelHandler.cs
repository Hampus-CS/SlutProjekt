using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ping = UnityEngine.Ping;

/// <summary>
/// Handles showing and hiding waiting and connecting panels during LAN matchmaking.
/// </summary>
public class WaitingPanelHandler : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject waitingPanel;      // Shown while hosting, waiting for player
    [SerializeField] private GameObject connectingPanel;   // Shown while joining, waiting for host response
    
    private void Start()
    {
        HideAllPanels();
    }
    
    /// <summary>
    /// Show the "Waiting for Opponent..." panel.
    /// </summary>
    public void ShowWaitingPanel()
    {
        if (waitingPanel != null)
            waitingPanel.SetActive(true);

        if (connectingPanel != null)
            connectingPanel.SetActive(false);
    }

    /// <summary>
    /// Show the "Connecting to Host..." panel.
    /// </summary>
    public void ShowConnectingPanel()
    {
        if (waitingPanel != null)
            waitingPanel.SetActive(false);

        if (connectingPanel != null)
            connectingPanel.SetActive(true);
    }

    /// <summary>
    /// Hide all panels (used after successful connection or when transitioning scenes).
    /// </summary>
    public void HideAllPanels()
    {
        if (waitingPanel != null)
            waitingPanel.SetActive(false);

        if (connectingPanel != null)
            connectingPanel.SetActive(false);
    }
    
}
