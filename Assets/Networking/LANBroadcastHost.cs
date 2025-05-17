using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using Unity.Netcode;
using TMPro;

/// <summary>
/// Broadcasts the host's IP address over the LAN periodically for auto-discovery.
/// Broadcast frequency decreases over time and stops when a client connects or host shuts down.
/// </summary>
public class LANBroadcastHost : MonoBehaviour
{
    [Header("Broadcast Settings")]
    [SerializeField] private int broadcastPort = 47777;

    private UdpClient udpClient;
    [SerializeField] private float timer;
    [SerializeField] private float elapsedTime;
    [SerializeField] private float currentInterval = 3f; // Start with 3 seconds

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI broadcastStatusText;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseSpeed = 2f; // How fast it pulses
    [SerializeField] private float pulseStrength = 2f; // How much the font size changes

    private float baseFontSize; // To remember original font size

    private void Start()
    {
        udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;
        Debug.Log("[LANBroadcastHost] Broadcast started.");

        if (broadcastStatusText != null)
            baseFontSize = broadcastStatusText.fontSize;
    }

    private void Update()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            Debug.Log("[LANBroadcastHost] Host stopped. Stopping broadcast.");
            StopBroadcasting();
            return;
        }

        if (NetworkManager.Singleton.ConnectedClientsList.Count >= 2)
        {
            Debug.Log("[LANBroadcastHost] Client connected! Stopping broadcast.");
            StopBroadcasting();
            return;
        }

        elapsedTime += Time.deltaTime;
        timer += Time.deltaTime;

        UpdateBroadcastInterval();
        UpdateBroadcastStatusText();
        UpdatePulseEffect();

        if (timer >= currentInterval)
        {
            timer = 0f;
            BroadcastHostInfo();
        }
    }

    private void UpdateBroadcastInterval()
    {
        if (elapsedTime < 30f)
        {
            currentInterval = 3f; // 0-30s: every 3 seconds
        }
        else if (elapsedTime < 120f)
        {
            currentInterval = 10f; // 30-120s: every 10 seconds
        }
        else
        {
            currentInterval = 30f; // After 2min: every 30 seconds
        }
    }

    private void BroadcastHostInfo()
    {
        string hostIP = GetLocalIPAddress();
        string message = $"FIGHTHOST:{hostIP}";
        byte[] data = Encoding.UTF8.GetBytes(message);

        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ni.OperationalStatus != OperationalStatus.Up)
                continue;

            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily != AddressFamily.InterNetwork)
                    continue;

                IPAddress broadcastAddress = GetBroadcastAddress(ip.Address, ip.IPv4Mask);
                if (broadcastAddress == null)
                    continue;

                try
                {
                    IPEndPoint endPoint = new IPEndPoint(broadcastAddress, broadcastPort);
                    udpClient.Send(data, data.Length, endPoint);
                    Debug.Log($"[LANBroadcastHost] Broadcasting IP: {hostIP} to {broadcastAddress}");
                }
                catch (SocketException ex)
                {
                    Debug.LogWarning($"[LANBroadcastHost] Failed to send broadcast to {broadcastAddress}: {ex.Message}");
                }
            }
        }
    }

    private IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
    {
        byte[] ipBytes = address.GetAddressBytes();
        byte[] maskBytes = subnetMask.GetAddressBytes();

        if (ipBytes.Length != maskBytes.Length)
            return null;

        byte[] broadcastBytes = new byte[ipBytes.Length];
        for (int i = 0; i < ipBytes.Length; i++)
        {
            broadcastBytes[i] = (byte)(ipBytes[i] | (maskBytes[i] ^ 255));
        }
        return new IPAddress(broadcastBytes);
    }

    private void StopBroadcasting()
    {
        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }

        enabled = false; // Disable Update loop
    }

    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
            {
                if (!ip.ToString().StartsWith("169.") && !ip.ToString().StartsWith("127.") && !ip.ToString().StartsWith("192.168.56.")) // Exclude VirtualBox, Loopback
                {
                    return ip.ToString();
                }
            }
        }
        return "127.0.0.1";
    }

    private void OnDestroy()
    {
        StopBroadcasting();
    }

    private void UpdateBroadcastStatusText()
    {
        if (broadcastStatusText != null)
        {
            int elapsedSeconds = Mathf.FloorToInt(elapsedTime);
            int intervalSeconds = Mathf.FloorToInt(currentInterval);

            string colorTag;

            if (elapsedSeconds < 30)
            {
                colorTag = "<color=green>";
            }
            else if (elapsedSeconds < 120)
            {
                colorTag = "<color=yellow>";
            }
            else
            {
                colorTag = "<color=red>";
            }

            broadcastStatusText.text = $"{colorTag}Elapsed: {elapsedSeconds}s\nBroadcast every: {intervalSeconds}s</color>";
        }
    }

    private void UpdatePulseEffect()
    {
        if (broadcastStatusText != null)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed) * pulseStrength;
            broadcastStatusText.fontSize = baseFontSize + pulse;
        }
    }
}
