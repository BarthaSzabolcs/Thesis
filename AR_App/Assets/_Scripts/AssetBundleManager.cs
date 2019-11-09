using Repository;
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

    #endregion
    #region Hide in editor

    public static AssetBundleManager Instance { get; private set; }
    public Dictionary<string, AssetBundle> Loaded { get; set; } = new Dictionary<string, AssetBundle>();

    private string AssetBundleCachePath => Path.Combine(Application.persistentDataPath, "AssetBundles");

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            Directory.CreateDirectory(AssetBundleCachePath);
        }
    }
    
    #endregion

    // ToDo - Handle multiple loads of the same bundle
    public IEnumerator Load(DataModels.AssetBundle assetBundleInfo)
    {
        UILog.Instance.WriteLn($"Load AssetBundle { assetBundleInfo.Name }.");

        if (Loaded.ContainsKey(assetBundleInfo.Name))
        {
            UILog.Instance.WriteLn($"AssetBundle { assetBundleInfo.Name } is already loaded.", Color.green);
            yield return null;
        }

        string url = Path.Combine(AssetBundleCachePath, assetBundleInfo.Name);
        if (File.Exists(url) == false) // ToDo - and there is connection, else could not load
        {
            UILog.Instance.WriteLn(url + " checked. Cached file not found.");

#if UNITY_ANDROID && !UNITY_EDITOR
            url = "{0}/Api/AssetBundle/{1}/File?platform=1";
#else
            url = "{0}/Api/AssetBundle/{1}/File?platform=0";
#endif
            url = string.Format(url, ConnectionManager.Instance.Con, assetBundleInfo.Id);

            UILog.Instance.WriteLn($"Download file from url:\n{url}");
            var apiRequest = UnityWebRequest.Get(url);
            yield return apiRequest.SendWebRequest();

            url = Path.Combine(AssetBundleCachePath, assetBundleInfo.Name);

            var recievedData = apiRequest.downloadHandler.data;
            if (recievedData.Length > 0)
            {

                File.WriteAllBytes(url, recievedData);
                UILog.Instance.WriteLn("Save file to local cache: " + url, Color.green);
            }
            else
            {
                UILog.Instance.WriteLn("File not found on the server", Color.red);
                yield return null;
            }
        }
        else
        {
            UILog.Instance.WriteLn(url + " checked. Cached file found.", Color.green);
        }

#if UNITY_ANDROID
        url = "file://" + url;
#endif

        UILog.Instance.WriteLn($"Load AssetBundle { assetBundleInfo.Name } from memory:\n{url}");
        var memoryRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return memoryRequest.SendWebRequest();

        var assetBundle = DownloadHandlerAssetBundle.GetContent(memoryRequest);

        if (assetBundle != null)
        {
            Loaded.Add(assetBundleInfo.Name, assetBundle);
            UILog.Instance.WriteLn($"Load of AssetBundle { assetBundleInfo.Name } successful.", Color.green);
        }
        else
        {
            UILog.Instance.WriteLn($"Load of AssetBundle { assetBundleInfo.Name } failed.", Color.red);
        }
    }

}
