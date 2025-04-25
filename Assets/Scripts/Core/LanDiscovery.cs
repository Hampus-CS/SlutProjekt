using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class LanDiscovery : MonoBehaviour
{
    public int discoveryPort = 8888;
    private UdpClient listener;
    private bool isListening = false;

    public async void StartListening(System.Action<string> onDiscovered)
    {
        if (isListening) return;
        isListening = true;

        try
        {
            listener = new UdpClient(discoveryPort);
            Debug.Log($"[LANDiscovery] Listening on UDP port {discoveryPort}");

            while (true)
            {
                UdpReceiveResult result;

                try
                {
                    result = await listener.ReceiveAsync();
                }
                catch (ObjectDisposedException)
                {
                    // Listener was closed, exit the loop
                    Debug.Log("[LANDiscovery] Listener closed.");
                    break;
                }
                catch (SocketException e)
                {
                    Debug.LogError($"[LANDiscovery] Socket error: {e.Message}");
                    break;
                }

                string message = Encoding.UTF8.GetString(result.Buffer);

                if (message == "DISCOVER_HOST")
                {
                    string response = "HOST_HERE";
                    byte[] data = Encoding.UTF8.GetBytes(response);

                    await listener.SendAsync(data, data.Length, result.RemoteEndPoint);
                    Debug.Log($"[LANDiscovery] Replied to {result.RemoteEndPoint}");
                }
                else if (message.StartsWith("HOST_HERE"))
                {
                    string hostIp = result.RemoteEndPoint.Address.ToString();

                    if (hostIp != "127.0.0.1") // prevent echoing own reply
                    {
                        Debug.Log($"[LANDiscovery] Discovered host at {hostIp}");

                        onDiscovered?.Invoke(hostIp);

                        // ✅ Stop listening after discovering the host
                        StopListening();
                        break;
                    }
                }
            }
        }
        catch (SocketException ex)
        {
            Debug.LogError($"[LANDiscovery] Failed to bind to port {discoveryPort}: {ex.Message}");
        }
    }

    public async void SendDiscoveryRequest()
    {
        using (UdpClient sender = new UdpClient())
        {
            sender.EnableBroadcast = true;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, discoveryPort);

            byte[] data = Encoding.UTF8.GetBytes("DISCOVER_HOST");
            try
            {
                await sender.SendAsync(data, data.Length, endPoint);
                Debug.Log("[LANDiscovery] Broadcasted discovery message");
            }
            catch (SocketException ex)
            {
                Debug.LogError($"[LANDiscovery] Broadcast failed: {ex.Message}");
            }
        }
    }
    
    public void StopListening()
    {
        if (listener != null)
        {
            listener.Close();
            listener = null;
        }
        isListening = false;
        Debug.Log("[LANDiscovery] Discovery stopped.");
    }

    private void OnApplicationQuit()
    {
        StopListening();
    }
}