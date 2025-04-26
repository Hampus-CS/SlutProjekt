using System;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages LAN hosting and joining.
/// </summary>
public class LANGameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject connectingPanel;
    [SerializeField] private WaitingPanelHandler waitingHandler;

    [Header("Connection Settings")]
    [SerializeField] private ushort port = 7777; // Port must be ushort instead of int because UnityTransport requires ushort and network ports are 16-bit values.

    private UnityTransport transport;

    private void Start()
    {
        transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        if (hostButton != null)
            hostButton.onClick.AddListener(OnHostClicked);

        if (joinButton != null)
            joinButton.onClick.AddListener(OnJoinClicked);
    }

    private void OnHostClicked()
    {
        StartHost();
    }

    private void OnJoinClicked()
    {
        StartClient();
    }

    private void StartHost()
    {
        transport.SetConnectionData("0.0.0.0", port);
        NetworkManager.Singleton.StartHost();

        if (waitingHandler != null)
            waitingHandler.ShowWaitingPanel();

        // NEW: Show own IP after starting host
        HostIPDisplay ipDisplay = FindObjectOfType<HostIPDisplay>();
        if (ipDisplay != null)
        {
            ipDisplay.ShowOwnIP();
        }

        Debug.Log("[LANGameManager] Hosting LAN game...");
    }

    private void StartClient()
    {
        string ipAddress = "127.0.0.1"; // Default fallback IP

        if (ipInputField != null && !string.IsNullOrEmpty(ipInputField.text))
        {
            // Use manually entered IP
            ipAddress = ipInputField.text;
        }
        else
        {
            // Try to use detected LAN broadcast IP
            LANBroadcastClient broadcastClient = FindObjectOfType<LANBroadcastClient>();
            if (broadcastClient != null && !string.IsNullOrEmpty(broadcastClient.detectedHostIP))
            {
                ipAddress = broadcastClient.detectedHostIP;
            }
        }

        transport.SetConnectionData(ipAddress, port);
        NetworkManager.Singleton.StartClient();

        if (waitingHandler != null)
            waitingHandler.ShowConnectingPanel();

        Debug.Log($"[LANGameManager] Attempting to join host at {ipAddress}...");
    }
}
