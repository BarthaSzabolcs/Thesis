using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleManager : MonoBehaviour
{
    #region Hide in editor

    public static AssetBundleManager Instance { get; private set; }
    private string CachePath => Path.Combine(Application.persistentDataPath, "AssetBundles");
    public Dictionary<string, AssetBundle> Loaded { get; set; } = new Dictionary<string, AssetBundle>();

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
        // UILog.Instance.WriteLn($"Load AssetBundle { assetBundleInfo.Name }.");

        if (Loaded.ContainsKey(assetBundleModel.Name))
        {
            UILog.Instance.WriteLn($"AssetBundle { assetBundleModel.Name } is already loaded.", Color.green);
            yield break;
        }

        yield return FetchFile(assetBundleModel);

        //        string url = Path.Combine(CachePath, assetBundleModel.Name);
        //        if (File.Exists(url) == false) // ToDo - and there is connection, else could not load
        //        {
        //            // UILog.Instance.WriteLn(url + " checked. Cached file not found.");

        //#if UNITY_ANDROID && !UNITY_EDITOR
        //            url = "{0}/Api/AssetBundle/{1}/File?platform=1";
        //#else
        //            url = "{0}/Api/AssetBundle/{1}/File?platform=0";
        //#endif
        //            url = string.Format(url, ConnectionManager.Instance.ApiUrl, assetBundleModel.Id);

        //            // UILog.Instance.WriteLn($"Download file from url:\n{ url }"); //ToDo - DownloadFile
        //            var apiRequest = UnityWebRequest.Get(url);
        //            yield return apiRequest.SendWebRequest();

        //            url = Path.Combine(CachePath, assetBundleModel.Name);

        //            var recievedData = apiRequest.downloadHandler.data;
        //            if (recievedData.Length > 0)
        //            {

        //                File.WriteAllBytes(url, recievedData);
        //                // UILog.Instance.WriteLn("Save file to local cache: " + url, Color.green);
        //            }
        //            else
        //            {
        //                // UILog.Instance.WriteLn("File not found on the server", Color.red);
        //                yield break;
        //            }
        //        }
        //        else
        //        {
        //            // UILog.Instance.WriteLn(url + " checked. Cached file found.", Color.green);
        //        }

        yield return LoadFromMemory(assetBundleModel);
    }
    private IEnumerator FetchFile(DataModels.AssetBundle assetBundle)
    {
        string url = Path.Combine(CachePath, assetBundle.Name);
        if (File.Exists(url) == false && Application.internetReachability != NetworkReachability.NotReachable)
        {
            UILog.Instance.WriteLn(url + " checked. Cached file not found.");

#if UNITY_ANDROID && !UNITY_EDITOR
            url = "{ ConnectionManager.Instance.ApiUrl }/Api/AssetBundle/{ assetBundle.Id }/File?platform=1";
#else
            url = "{ ConnectionManager.Instance.ApiUrl }/Api/AssetBundle/{ assetBundle.Id }/File?platform=0";
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
    private IEnumerator LoadFromMemory(DataModels.AssetBundle assetBundleModel)
    {
        var url = Path.Combine(CachePath, assetBundleModel.Name);

        #if UNITY_ANDROID
        url = "file://" + url;
        #endif

        // UILog.Instance.WriteLn($"Load AssetBundle { assetBundleModel.Name } from memory:\n{ url }");
        var memoryRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return memoryRequest.SendWebRequest();

        var assetBundle = DownloadHandlerAssetBundle.GetContent(memoryRequest);

        if (assetBundle != null)
        {
            Loaded.Add(assetBundleModel.Name, assetBundle);
            // UILog.Instance.WriteLn($"Load of AssetBundle { assetBundleModel.Name } successful.", Color.green);
        }
        else
        {
            // UILog.Instance.WriteLn($"Load of AssetBundle { assetBundleModel.Name } failed.", Color.red);
        }
    }

}
//    // ToDo - Handle multiple loads of the same bundle
//    public IEnumerator Load(DataModels.AssetBundle assetBundleInfo)
//    {
//        UILog.Instance.WriteLn($"Load AssetBundle { assetBundleInfo.Name }.");

//        if (Loaded.ContainsKey(assetBundleInfo.Name))
//        {
//            UILog.Instance.WriteLn($"AssetBundle { assetBundleInfo.Name } is already loaded.", Color.green);
//            yield return null;
//        }

//        string url = Path.Combine(CachePath, assetBundleInfo.Name);
//        if (File.Exists(url) == false) // ToDo - and there is connection, else could not load
//        {
//            UILog.Instance.WriteLn(url + " checked. Cached file not found.");

//#if UNITY_ANDROID && !UNITY_EDITOR
//            url = "{0}/Api/AssetBundle/{1}/File?platform=1";
//#else
//            url = "{0}/Api/AssetBundle/{1}/File?platform=0";
//#endif
//            url = string.Format(url, ConnectionManager.Instance.Con, assetBundleInfo.Id);

//            UILog.Instance.WriteLn($"Download file from url:\n{url}");
//            var apiRequest = UnityWebRequest.Get(url);
//            yield return apiRequest.SendWebRequest();

//            url = Path.Combine(CachePath, assetBundleInfo.Name);

//            var recievedData = apiRequest.downloadHandler.data;
//            if (recievedData.Length > 0)
//            {

//                File.WriteAllBytes(url, recievedData);
//                UILog.Instance.WriteLn("Save file to local cache: " + url, Color.green);
//            }
//            else
//            {
//                UILog.Instance.WriteLn("File not found on the server", Color.red);
//                yield return null;
//            }
//        }
//        else
//        {
//            UILog.Instance.WriteLn(url + " checked. Cached file found.", Color.green);
//        }

//#if UNITY_ANDROID
//        url = "file://" + url;
//#endif

//        UILog.Instance.WriteLn($"Load AssetBundle { assetBundleInfo.Name } from memory:\n{url}");
//        var memoryRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
//        yield return memoryRequest.SendWebRequest();

//        var assetBundle = DownloadHandlerAssetBundle.GetContent(memoryRequest);

//        if (assetBundle != null)
//        {
//            Loaded.Add(assetBundleInfo.Name, assetBundle);
//            UILog.Instance.WriteLn($"Load of AssetBundle { assetBundleInfo.Name } successful.", Color.green);
//        }
//        else
//        {
//            UILog.Instance.WriteLn($"Load of AssetBundle { assetBundleInfo.Name } failed.", Color.red);
//        }
//    }


    // ToDo - Handle multiple loads of the same bundle


