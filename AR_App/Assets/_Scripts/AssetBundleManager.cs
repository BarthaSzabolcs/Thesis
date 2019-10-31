using DataAcces.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleManager : MonoBehaviour
{
    #region Show in editor

    [Header("Test:")]
    [SerializeField] string wantedAssetBundleName;
    [SerializeField] int wantedAssetBundleId;

    [Header("Connection:")]
    [SerializeField] private ConnectionConfig con;

    #endregion
    #region Hide in editor

    public static AssetBundleManager Instance { get; private set; }
    public Dictionary<string, AssetBundle> Loaded { get; set; } = new Dictionary<string, AssetBundle>();

    private string CachePath => Path.Combine(Application.persistentDataPath, "AssetBundle");
    
    #endregion

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            Directory.CreateDirectory(CachePath);
            
            DontDestroyOnLoad(gameObject);
        }
    }

    // ToDo - Handle multiple loads of the same bundle
    public IEnumerator Load(DataAcces.DataModels.FileInfo fileInfo)
    {
        if (Loaded.ContainsKey(fileInfo.Name))
        {
            yield return null;
        }

        string url = Path.Combine(CachePath, fileInfo.Name);
        if (File.Exists(url) == false)
        {
            Debug.Log(url + " checked. File not found.");

            url = string.Format("{0}/Api/File/{1}", con.server, fileInfo.Id);

            Debug.Log("Download file from API: " + url);
            var apiRequest = UnityWebRequest.Get(url);
            yield return apiRequest.SendWebRequest();

            url = Path.Combine(CachePath, fileInfo.Name);

            Debug.Log("Save file to local cache: " + url);
            File.WriteAllBytes(url, apiRequest.downloadHandler.data);
        }
        else
        {
            Debug.Log(url + " checked. File found.");
        }

        Debug.Log("Load file from memory: " + url);
        var memoryRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return memoryRequest.SendWebRequest();

        var assetBundle = DownloadHandlerAssetBundle.GetContent(memoryRequest);

        if (assetBundle != null)
        {
            Loaded.Add(fileInfo.Name, assetBundle);
            Debug.Log("File load successful.");
        }
        else
        {
            Debug.Log("File load failed.");
        }
    }

}
