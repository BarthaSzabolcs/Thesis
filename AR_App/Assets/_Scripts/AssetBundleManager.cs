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

    private string CachePath => Path.Combine(Application.persistentDataPath, "AssetBundles");
    
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
    public IEnumerator Load(DataModels.AssetBundle assetBundleInfo)
    {
        if (Loaded.ContainsKey(assetBundleInfo.Name))
        {
            yield return null;
        }

        string url = Path.Combine(CachePath, assetBundleInfo.Name);
        if (File.Exists(url) == false)
        {
            Debug.Log(url + " checked. File not found.");

            url = string.Format("{0}/Api/AssetBundle/{1}/File", con.server, assetBundleInfo.Id);

            Debug.Log("Download file from API: " + url);
            var apiRequest = UnityWebRequest.Get(url);
            yield return apiRequest.SendWebRequest();

            url = Path.Combine(CachePath, assetBundleInfo.Name);

            var recievedData = apiRequest.downloadHandler.data;
            if (recievedData.Length > 0)
            {

                File.WriteAllBytes(url, recievedData);
                Debug.Log("Save file to local cache: " + url);
            }
            else
            {
                Debug.Log("File not found on the server");
                yield return null;
            }
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
            Loaded.Add(assetBundleInfo.Name, assetBundle);
            Debug.Log("File load successful.");
        }
        else
        {
            Debug.Log("File load failed.");
        }
    }

}
