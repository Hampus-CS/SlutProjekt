using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Displays your own host IP address and allows copying it to the clipboard when clicked.
/// Onlyshown after hosting starts.
/// </summary>
public class HostIPDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI hostIPText;

    private string currentHostIP = "";

    private void Start()
    {
        // Start with hidden or empty state
        if (hostIPText != null)
        {
            hostIPText.text = "";
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Should be called after hosting starts to show the IP.
    /// </summary>
    public void ShowOwnIP()
    {
        currentHostIP = GetLocalIPAddress();

        if (hostIPText != null)
        {
            hostIPText.text = $"Your IP: {currentHostIP}\n(click to copy)";
            gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Copies the current host IP address to the clipboard.
    /// </summary>
    public void CopyIPToClipboard()
    {
        if (!string.IsNullOrEmpty(currentHostIP))
        {
            GUIUtility.systemCopyBuffer = currentHostIP;
            Debug.Log($"[HostIPDisplay] Copied {currentHostIP} to clipboard.");
        }
    }

    /// <summary>
    /// Gets the real local LAN IP address, excluding VirtualBox or loopbacks.
    /// </summary>
    /// <returns>Local IP address as string.</returns>
    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
            {
                if (!ip.ToString().StartsWith("169.") && !ip.ToString().StartsWith("127.") && !ip.ToString().StartsWith("192.168.56."))
                {
                    return ip.ToString();
                }
            }
        }
        return "127.0.0.1"; // Fallback if no good IP found
    }
}
