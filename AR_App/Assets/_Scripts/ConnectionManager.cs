using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private ConnectionConfig connectionConfig;
    [SerializeField] private string relativeCachePath;

    public static ConnectionManager Instance { get; set; }

    public string ApiUrl { get; private set; }
    public SqliteConnection CacheConnection { get; private set; }
    public AccesMode Mode { get; set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            ApiUrl = connectionConfig.server;
            CacheConnection = new SqliteConnection("Data Source=" + Path.Combine(Application.persistentDataPath, relativeCachePath));

            ipInputField.text = ApiUrl;
        }
        else
        {
            Destroy(this);
        }
    }

    public void RefreshConnection()
    {
        ApiUrl = ipInputField.text;
    }

    // ToDo - Test API acces
    public void TestApiAcces()
    {
        // Mode = AccesMode.Offline;
    }
}
