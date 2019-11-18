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
    #region Show in editor

    [SerializeField] private ConnectionConfig connectionConfig;
    [SerializeField] private string relativeCachePath;
    [SerializeField] private int timeoutInSeconds;

    #endregion
    #region Hide in editor

    public static ConnectionManager Instance { get; set; }
    public string ApiUrl { get; private set; }
    public SqliteConnection CacheConnection { get; private set; }
    public bool ApiReachable { get; set; }

    #endregion

    #region Unity Callbacks

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            ApiUrl = connectionConfig.server;
            CacheConnection = new SqliteConnection("Data Source=" + Path.Combine(Application.persistentDataPath, relativeCachePath));
        }
        else
        {
            Destroy(this);
        }
    }
    
    #endregion

    public IEnumerator TestApiAcces()
    {
        var testRequest = UnityWebRequest.Get(ApiUrl);
        testRequest.timeout = timeoutInSeconds;

        yield return testRequest.SendWebRequest();

        ApiReachable = testRequest.error is null;
    }
}