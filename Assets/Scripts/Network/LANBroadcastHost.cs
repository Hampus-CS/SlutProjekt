using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Broadcasts the host's IP address over the LAN periodically for auto-discovery.
/// </summary>
public class LANBroadcastHost : MonoBehaviour
{
    [Header("Broadcast Settings")]
    [SerializeField] private int broadcastPort = 47777;
    [SerializeField] private float broadcastInterval = 1f; // seconds

    private UdpClient udpClient;
    private float timer;

    private void Start()
    {
        udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= broadcastInterval)
        {
            timer = 0f;
            BroadcastHostInfo();
        }
    }

    private void BroadcastHostInfo()
    {
        string hostIP = GetLocalIPAddress();
        string message = $"FIGHTHOST:{hostIP}";
        byte[] data = Encoding.UTF8.GetBytes(message);

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
        udpClient.Send(data, data.Length, endPoint);

        Debug.Log($"[LANBroadcastHost] Broadcasting IP: {hostIP}");
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
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}
