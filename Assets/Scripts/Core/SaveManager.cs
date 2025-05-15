using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Unity.Netcode;
using UnityEngine;
//using Unity.Netcode;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private readonly string BaseFileName = "player_stats";
    private readonly string FileExt = ".dat";
    private readonly string encryptionKey = "temp1234"; // Generate a stronger one at a later moment

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Save the result of a single match for this local client.
    /// Each client writes to its own file named player_stats_{clientId}.dat
    /// </summary>
    public void SaveMatchResult(int kills, int deaths, bool won)
    {
        try
        {
            ulong clientId = NetworkManager.Singleton?.LocalClientId ?? 0;
            string path = GetSavePath(clientId);

            // load existing data or create new
            PlayerSaveData data = File.Exists(path) ? JsonUtility.FromJson<PlayerSaveData>(Decrypt(File.ReadAllText(path))) : new PlayerSaveData();

            // register this match
            data.RegisterMatch(kills, deaths, won);

            // serialize, encrypt, write
            string json = JsonUtility.ToJson(data);
            string encrypted = Encrypt(json);
            File.WriteAllText(path, encrypted);

            Debug.Log($"[SaveManager] (Client {clientId}) Saved stats: +{kills}k +{deaths}d won:{won}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveManager] Save failed: {ex}");
        }
    }

    /// <summary>
    /// Load the cumulative stats for this local client.
    /// </summary>
    public PlayerSaveData LoadStats()
    {
        try
        {
            ulong clientId = NetworkManager.Singleton?.LocalClientId ?? 0;
            string path = GetSavePath(clientId);
            if (!File.Exists(path))
                return new PlayerSaveData();

            string encrypted = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerSaveData>(Decrypt(encrypted));
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveManager] Load failed: {ex}");
            return new PlayerSaveData();
        }
    }
    
    private string GetSavePath(ulong clientId)
    {
        string fileName = $"{BaseFileName}_{clientId}{FileExt}";
        return Path.Combine(Application.persistentDataPath, fileName);
    }
    
    // Encryption using AES
    private string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32));
        aes.IV = new byte[16]; // Zero IV for simplicity (consider random IV + prepended for production)

        using var encryptor = aes.CreateEncryptor();
        byte[] input = Encoding.UTF8.GetBytes(plainText);
        byte[] encrypted = encryptor.TransformFinalBlock(input, 0, input.Length);
        return Convert.ToBase64String(encrypted);
    }

    private string Decrypt(string encryptedText)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(encryptionKey.PadRight(32));
        aes.IV = new byte[16];

        using var decryptor = aes.CreateDecryptor();
        byte[] input = Convert.FromBase64String(encryptedText);
        byte[] decrypted = decryptor.TransformFinalBlock(input, 0, input.Length);
        return Encoding.UTF8.GetString(decrypted);
    }
}
