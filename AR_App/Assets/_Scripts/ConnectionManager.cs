using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private ConnectionConfig connectionConfig;
    [SerializeField] private string relativeCachePath;

    public static ConnectionManager Instance { get; set; }

    public string ApiUrl { get; private set; }
    public SqliteConnection CacheConnection { get; private set; }
    public ApiDataAcces ApiDataAccesMode { get; set; }

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

    public IEnumerator TestApiAcces()
    {
        var testRequest = UnityWebRequest.Get(ApiUrl);

        yield return testRequest.SendWebRequest();

        if (testRequest.error != null)
        {
            ApiDataAccesMode = ApiDataAcces.Offline;
            UILog.Instance.WriteLn("API offline.");
        }
        else
        {
            ApiDataAccesMode = ApiDataAcces.Online;
            UILog.Instance.WriteLn("API online.");
        }
    }
}