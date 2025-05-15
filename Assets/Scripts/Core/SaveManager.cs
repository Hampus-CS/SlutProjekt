using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Manages saving and loading encrypted player data per local profile.
/// </summary>
public class SaveManager : MonoBehaviour
{

    [Header("Save Settings")]
    [SerializeField] private static readonly string SaveExtension = ".dat";
    [SerializeField] private static string encryptionKey = "ThisKeyIsTotallyUnbreakable1234!"; // Must be 32 characters for AES-256, ignore what it is called ;)
    [SerializeField] private static bool useEncryption = true;

    private static string currentProfile = "Player1"; // default fallback
    
    /// <summary>Set which player profile we're saving/loading for.</summary>
    public static void SetCurrentProfile(string profileName)
    {
        currentProfile = profileName;
    }

    /// <summary>Set a new encryption key (must be 32 characters for AES-256).</summary>
    public static void SetEncryptionKey(string key)
    {
        if (key.Length != 32)
            Debug.LogWarning("[SaveManager] AES key must be exactly 32 characters!");
        else
            encryptionKey = key;
    }

    /// <summary>Enable or disable encryption globally.</summary>
    public static void SetEncryptionEnabled(bool enabled)
    {
        useEncryption = enabled;
    }

    public static void SavePlayerData(PlayerSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string filePath = GetSaveFilePath();

        try
        {
            if (useEncryption)
            {
                byte[] encrypted = Encrypt(json);
                File.WriteAllBytes(filePath, encrypted);
                Debug.Log($"[SaveManager] Saved encrypted data to: {filePath}");
            }
            else
            {
                File.WriteAllText(filePath, json);
                Debug.Log($"[SaveManager] Saved plain data to: {filePath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to save data: {e.Message}");
        }
    }

    public static PlayerSaveData LoadPlayerData()
    {
        string filePath = GetSaveFilePath();

        if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
        {
            Debug.Log($"[SaveManager] No valid save file found. Starting fresh.");
            var fresh = new PlayerSaveData();
            SavePlayerData(fresh);
            return fresh;
        }

        try
        {
            if (useEncryption)
            {
                byte[] encrypted = File.ReadAllBytes(filePath);
                string decryptedJson = Decrypt(encrypted);
                return JsonUtility.FromJson<PlayerSaveData>(decryptedJson);
            }
            else
            {
                string json = File.ReadAllText(filePath);
                return JsonUtility.FromJson<PlayerSaveData>(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Failed to load data: {e.Message}");
            return new PlayerSaveData();
        }
    }

    public static void DeleteSave()
    {
        string filePath = GetSaveFilePath();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log($"[SaveManager] Deleted save file: {filePath}");
        }
    }

    private static string GetSaveFilePath()
    {
        string fileName = $"player_stats_{currentProfile}{SaveExtension}";
        return Path.Combine(Application.persistentDataPath, fileName);
    }

    // AES Encryption
    private static byte[] Encrypt(string plainText)
    {
	    byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
	    if (keyBytes.Length != 32)
	    {
		    Debug.LogError($"[SaveManager] Invalid AES key length: {keyBytes.Length} bytes");
		    return Array.Empty<byte>();
	    }

	    using Aes aes = Aes.Create();
	    aes.Key = keyBytes;
	    aes.IV = new byte[16]; // Static IV is OK for local encrypted saves

	    using MemoryStream ms = new MemoryStream();
	    using CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
	    using StreamWriter sw = new StreamWriter(cs, Encoding.UTF8)
	    {
		    AutoFlush = true
	    };

	    sw.Write(plainText);
	    sw.Flush();
	    cs.FlushFinalBlock();

	    Debug.Log($"[SaveManager] Encrypted {ms.ToArray().Length} bytes");

	    return ms.ToArray();
    }

    private static string Decrypt(byte[] cipherBytes)
    {
	    byte[] keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
	    if (keyBytes.Length != 32)
	    {
		    Debug.LogError($"[SaveManager] Invalid AES key length: {keyBytes.Length} bytes");
		    return null;
	    }

	    using Aes aes = Aes.Create();
	    aes.Key = keyBytes;
	    aes.IV = new byte[16];

	    using MemoryStream ms = new MemoryStream(cipherBytes);
	    using CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
	    using StreamReader sr = new StreamReader(cs, Encoding.UTF8);
	    return sr.ReadToEnd();
    }
}
