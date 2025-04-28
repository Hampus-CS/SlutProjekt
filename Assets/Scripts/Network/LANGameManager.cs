using System;
using System.Collections;
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

    [Header("Broadcast References")]
    [SerializeField] private GameObject lanBroadcastHost;
    [SerializeField] private GameObject lanBroadcastClient;
    
    [Header("Connection Settings")]
    [SerializeField] private ushort port = 7777; // Port must be ushort instead of int because UnityTransport requires ushort and network ports are 16-bit values.

    private Coroutine searchCoroutine;
    
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
        // Open Windows-Firewall ports on Windows builds (UAC first run)
        #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            WindowsFirewallHelper.OpenHostPorts();
        #endif
        
        transport.SetConnectionData("0.0.0.0", port);
        NetworkManager.Singleton.StartHost();

        if (waitingHandler != null)
            waitingHandler.ShowWaitingPanel();

        // ACTIVATE LANBroadcastHost
        if (lanBroadcastHost != null)
        {
            lanBroadcastHost.SetActive(true);
        }
        
        // Show own IP after starting host
        HostIPDisplay ipDisplay = FindObjectOfType<HostIPDisplay>();
        if (ipDisplay != null)
        {
            ipDisplay.ShowOwnIP();
        }

        Debug.Log("[LANGameManager] Hosting LAN game...");
    }

    private void StartClient()
    {
        // Open Windows-Firewall ports on Windows builds (UAC first run)
        #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            WindowsFirewallHelper.OpenClientPorts();
        #endif
        
        if (waitingHandler != null)
            waitingHandler.ShowConnectingPanel();

        if (lanBroadcastClient != null)
            lanBroadcastClient.SetActive(true);

        string manualIP = ipInputField != null ? ipInputField.text : "";

        if (!string.IsNullOrEmpty(manualIP))
        {
            // Manual IP entered -> connect immediately
            ConnectToHost(manualIP);
        }
        else
        {
            // No manual IP entered -> start searching
            searchCoroutine = StartCoroutine(SearchForHost());
        }
    }

    private IEnumerator SearchForHost()
    {
        var broadcastClient = lanBroadcastClient.GetComponent<LANBroadcastClient>();

        Debug.Log("[LANGameManager] Searching for host...");

        float timeout = 30f;
        float elapsed = 0f;
        
        while (broadcastClient != null && string.IsNullOrEmpty(broadcastClient.detectedHostIP))
        {
            elapsed += Time.deltaTime;
            
            if (elapsed >= timeout)
            {
                Debug.LogWarning("[LANGameManager] Host search timed out. No host found.");
            
                // Optional: Show a "No Host Found" UI message here
                if (waitingHandler != null)
                    waitingHandler.HideAllPanels(); // Hide connecting panel if still showing

                yield break; // Stop searching
            }
            
            // Wait until detectedHostIP is filled
            yield return null;
        }

        if (broadcastClient != null && !string.IsNullOrEmpty(broadcastClient.detectedHostIP))
        {
            Debug.Log($"[LANGameManager] Host found at {broadcastClient.detectedHostIP}. Connecting...");
            ConnectToHost(broadcastClient.detectedHostIP);
        }
        else
        {
            Debug.LogWarning("[LANGameManager] No host found.");
        }
    }
    
    private void ConnectToHost(string ipAddress)
    {
            transport.SetConnectionData(ipAddress, port);
            NetworkManager.Singleton.StartClient();
            Debug.Log($"[LANGameManager] Attempting to join host at {ipAddress}...");
    }
}

