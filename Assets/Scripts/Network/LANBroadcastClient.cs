using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

/// <summary>
/// Listens for LAN broadcast messages to detect available hosts.
/// </summary>
public class LANBroadcastClient : MonoBehaviour
{
    [Header("Listening Settings")]
    [SerializeField] private int broadcastPort = 47777;

    private UdpClient udpListener;
    private Thread listenThread;
    private bool isListening = false;
    public string detectedHostIP = "";

    public delegate void HostFoundDelegate(string hostIP);
    public event HostFoundDelegate OnHostFound;

    private void Start()
    {
        if (udpListener != null)
        {
            Debug.LogWarning("[LANBroadcastClient] Listener already active. Skipping Start.");
            return;
        }
        
        try
        {
            // Important: Bind UDP listener to 0.0.0.0 (all interfaces)
            udpListener = new UdpClient(new IPEndPoint(IPAddress.Any, broadcastPort));
            udpListener.EnableBroadcast = true;

            isListening = true;
            listenThread = new Thread(ListenForBroadcast);
            listenThread.IsBackground = true;
            listenThread.Start();
            
            Debug.Log("[LANBroadcastClient] Started listening for broadcasts.");
        }
        catch (SocketException ex)
        {
            Debug.LogError($"[LANBroadcastClient] Failed to start listener: {ex.Message}");
        }
    }

    private void ListenForBroadcast()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, broadcastPort);

        while (isListening)
        {
	        try
	        {
		        byte[] data = udpListener.Receive(ref remoteEndPoint);
		        string message = Encoding.UTF8.GetString(data);

		        if (message.StartsWith("FIGHTHOST:"))
		        {
			        string hostIP = message.Substring(10);
			        detectedHostIP = hostIP;
			        Debug.Log($"[LANBroadcastClient] Host found at: {hostIP}");

			        // Fire event for UI or auto-join
			        OnHostFound?.Invoke(hostIP);
		        }
	        }
	        catch (SocketException ex)
	        {
		        Debug.LogWarning($"[LANBroadcastClient] Socket exception: {ex.Message}");
	        }
	        catch (System.Exception)
	        {
		        // swallow ThreadAbort or others on shutdown
	        }
        }
    }

    private void OnDestroy()
    {
        isListening = false;
        if (udpListener != null)
        {
            udpListener.Close();
            udpListener = null; // Important: Null it after closing
        }
        if (listenThread != null && listenThread.IsAlive)
        {
	        listenThread.Join(200);   // wait 0.2s then drop the reference
            listenThread = null; // Important: Null the thread reference
        }
    }
}
