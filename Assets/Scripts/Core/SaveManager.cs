using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Unity.Netcode;
using UnityEngine;
//using Unity.Netcode;

public class SaveManager : MonoBehaviour // WIP
{
    public static SaveManager Instance { get; private set; }

    private readonly string saveFileName = "player_save.dat";
    private readonly string encryptionKey = "temp1234"; // You can generate a stronger one

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
    /// Saves player data to an encrypted file (host or offline only).
    /// </summary>
    public void SavePlayerScore(string playerID, int score)
    {
        if (!IsHostOrOffline()) return;

        try
        {
            string path = GetSavePath();
            PlayerSaveData saveData = LoadAllData();
            saveData.SetScore(playerID, score);

            string json = JsonUtility.ToJson(saveData, prettyPrint: false);
            string encrypted = Encrypt(json);
            File.WriteAllText(path, encrypted);

            Debug.Log($"[SaveManager] Saved encrypted score for {playerID}: {score}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveManager] Save failed for {playerID}: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads the saved score for a specific player.
    /// </summary>
    public int LoadPlayerScore(string playerID)
    {
        try
        {
            PlayerSaveData saveData = LoadAllData();
            return saveData.GetScore(playerID);
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SaveManager] Load failed for {playerID}: {ex.Message}");
            return 0;
        }
    }

    // Internal: Loads and decrypts the full save data
    private PlayerSaveData LoadAllData()
    {
        string path = GetSavePath();
        if (!File.Exists(path))
        {
            return new PlayerSaveData(); // Return empty if no file
        }

        string encrypted = File.ReadAllText(path);
        string json = Decrypt(encrypted);
        return JsonUtility.FromJson<PlayerSaveData>(json);
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

    private string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, saveFileName);
    }

    private bool IsHostOrOffline()
    {
        if (NetworkManager.Singleton == null)
            return true;
        return NetworkManager.Singleton.IsHost || !NetworkManager.Singleton.IsClient;
    }

}
