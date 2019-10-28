using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleManager : MonoBehaviour
{
    #region Show in editor

    [Header("Connection:")]
    [SerializeField] private ConnectionConfig con;
    [SerializeField] private string cachePath;
    
    #endregion
    #region Hide in editor

    public AssetBundleManager Instance { get; set; }
    public Dictionary<string, AssetBundle> Loaded { get; set; }
    
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
            DontDestroyOnLoad(gameObject);
        }
    }

    public IEnumerator Load(string bundleName)
    {
        if (Loaded.ContainsKey(bundleName))
            yield return null;

        string url = Path.Combine(cachePath, bundleName);
        if (File.Exists(url) == false)
        {
            url = con.server + "/AssetBundle/" + bundleName;
        }

        var request = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return request.SendWebRequest();

        Loaded.Add(bundleName, DownloadHandlerAssetBundle.GetContent(request));
    }

    //IEnumerator InstantiateObject()
    //{
    //    GameObject canvas = bundle.LoadAsset<GameObject>("RecognizedObjectCanvas");
    //}
}
