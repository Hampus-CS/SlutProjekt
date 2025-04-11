using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
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

}
