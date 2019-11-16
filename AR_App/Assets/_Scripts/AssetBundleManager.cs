using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleManager : MonoBehaviour
{
    #region Hide in editor

    public static AssetBundleManager Instance { get; private set; }
    public Dictionary<string, AssetBundle> Loaded { get; set; } = new Dictionary<string, AssetBundle>();

    private string CachePath => Path.Combine(Application.persistentDataPath, "AssetBundles");

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

            Directory.CreateDirectory(CachePath);
        }
    }
    
    #endregion

    public IEnumerator Load(DataModels.AssetBundle assetBundleModel)
    {
        UILog.Instance.WriteLn($"Load AssetBundle { assetBundleModel.Name }.");

        if (Loaded.ContainsKey(assetBundleModel.Name))
        {
            UILog.Instance.WriteLn($"AssetBundle { assetBundleModel.Name } is already loaded.", Color.green);
            yield break;
        }

        yield return FetchFile(assetBundleModel);

        LoadFromMemory(assetBundleModel);
    }
    private IEnumerator FetchFile(DataModels.AssetBundle assetBundle)
    {
        string url = Path.Combine(CachePath, assetBundle.Name);
        if (File.Exists(url) == false)
        {
            UILog.Instance.WriteLn(url + " checked. Cached file not found.");

#if UNITY_ANDROID && !UNITY_EDITOR
            url = $"{ ConnectionManager.Instance.ApiUrl }/Api/AssetBundle/{ assetBundle.Id }/File?platform=1";
#else
            url = $"{ ConnectionManager.Instance.ApiUrl }/Api/AssetBundle/{ assetBundle.Id }/File?platform=0";
#endif
            UILog.Instance.WriteLn($"Download file from url:\n{ url }");
            var apiRequest = UnityWebRequest.Get(url);
            yield return apiRequest.SendWebRequest();

            url = Path.Combine(CachePath, assetBundle.Name);

            var recievedData = apiRequest.downloadHandler.data;
            if (recievedData.Length > 0)
            {

                File.WriteAllBytes(url, recievedData);
                UILog.Instance.WriteLn("Save file to local cache: " + url, Color.green);
            }
            else
            {
                UILog.Instance.WriteLn("File not found on the server", Color.red);
                yield break;
            }
        }
        else
        {
            UILog.Instance.WriteLn(url + " checked. Cached file found.", Color.green);
        }
    }
    private void LoadFromMemory(DataModels.AssetBundle assetBundleModel)
    {
        var url = Path.Combine(CachePath, assetBundleModel.Name);

        UILog.Instance.WriteLn($"Load AssetBundle { assetBundleModel.Name } from memory:\n{ url }");
        var assetBundle = AssetBundle.LoadFromFile(url);

        if (assetBundle != null)
        {
            Loaded.Add(assetBundleModel.Name, assetBundle);
            UILog.Instance.WriteLn($"Load of AssetBundle { assetBundleModel.Name } successful.", Color.green);
        }
        else
        {
            UILog.Instance.WriteLn($"Load of AssetBundle { assetBundleModel.Name } failed.", Color.red);
        }
    }
}