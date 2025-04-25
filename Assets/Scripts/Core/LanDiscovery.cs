using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class LanDiscovery : MonoBehaviour
{
    public int discoveryPort = 8888;
    private UdpClient listener;
    private bool isListening = false;

    public async void StartListening(System.Action<string> onDiscovered)
    {
        if (isListening) return;
        isListening = true;

        listener = new UdpClient(discoveryPort);
        while (true)
        {
            var result = await listener.ReceiveAsync();
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
                Debug.Log($"[LANDiscovery] Received host response from {result.RemoteEndPoint}");
                onDiscovered?.Invoke(result.RemoteEndPoint.Address.ToString());
            }
        }
    }

    public async void SendDiscoveryRequest()
    {
        using (UdpClient sender = new UdpClient())
        {
            sender.EnableBroadcast = true;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, discoveryPort);

            byte[] data = Encoding.UTF8.GetBytes("DISCOVER_HOST");
            await sender.SendAsync(data, data.Length, endPoint);
            Debug.Log("[LANDiscovery] Broadcasted discovery message");
        }
    }

    private void OnApplicationQuit()
    {
        listener?.Dispose();
    }
}