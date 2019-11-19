using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using CustomConsole;
using DataAcces;
using System;
using Newtonsoft.Json;

namespace AssetBundleManagment
{
    public class AssetBundleManager : MonoBehaviour
    {
        #region Hide in editor

        public static AssetBundleManager Instance { get; private set; }

        private Dictionary<int, AssetBundleInfo> assetBundleInfos = new Dictionary<int, AssetBundleInfo>();
        private string CachePath => Path.Combine(Application.persistentDataPath, "AssetBundles");
        private AssetBundleRepository repo;

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
                repo = new AssetBundleRepository();
            }
        }
        private void Start()
        {
            StartCoroutine(FetchAssetBundles());
        }
        #endregion

        private IEnumerator FetchAssetBundles()
        {
            yield return ConnectionManager.Instance.TestApiAcces();

            if (ConnectionManager.Instance.ApiReachable)
            {
                yield return FetchAssetBundlesFromApi();
            }

            FetchAssetBundlesFromCache();
        }
        private void FetchAssetBundlesFromCache()
        {
            var cache = repo.GetAll();

            foreach (var bundle in cache)
            {
                if (assetBundleInfos.TryGetValue(bundle.Id, out var info))
                {
                    info.Cache = bundle;
                }
                else
                {
                    assetBundleInfos.Add(bundle.Id, new AssetBundleInfo{ Cache = bundle });
                }
            }
        }
        private IEnumerator FetchAssetBundlesFromApi()
        {
            var url = $"{ ConnectionManager.Instance.ApiUrl }/Api/AssetBundle";
            var apiRequest = UnityWebRequest.Get(url);

            yield return apiRequest.SendWebRequest();

            var jsonResponse = apiRequest.downloadHandler.text;
            var onlineBundles = JsonConvert.DeserializeObject<List<DataModels.AssetBundle>>(jsonResponse);

            foreach (var bundle in onlineBundles)
            {
                if (assetBundleInfos.TryGetValue(bundle.Id, out var info))
                {
                    info.Api = bundle;
                }
                else
                {
                    assetBundleInfos.Add(bundle.Id, new AssetBundleInfo { Api = bundle });
                }
            }
        }

        public IEnumerator UseBundle(DataModels.AssetBundle model, Action<AssetBundle> callback)
        {
            if (assetBundleInfos.TryGetValue(model.Id, out var info))
            {
                if (info.Loaded != null)
                {
                    callback(info.Loaded);
                    yield break;
                }
                else
                {
                    info.LoadCallbacks.Add(callback);

                    if (info.LoadStarted == false)
                    {
                        yield return FetchFile(info);
                    }
                }
            }
        }
        private IEnumerator FetchFile(AssetBundleInfo info)
        {
            info.LoadStarted = true;

            if (ConnectionManager.Instance.ApiReachable && info.Syncronised == false)
            {
                yield return DownloadFile(info.Api);
            }

            var assetBundle = LoadFile(info.Prefered);

            while (info.LoadCallbacks.Count > 0)
            {
                info.LoadCallbacks[0](assetBundle);
                info.LoadCallbacks.RemoveAt(0);
            }
        }
        private IEnumerator DownloadFile(DataModels.AssetBundle assetBundle)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            string url = $"{ ConnectionManager.Instance.ApiUrl }/Api/AssetBundle/{ assetBundle.Id }/File?platform=1";
#else
            string url = $"{ ConnectionManager.Instance.ApiUrl }/Api/AssetBundle/{ assetBundle.Id }/File?platform=0";
#endif
            var apiRequest = UnityWebRequest.Get(url);
            yield return apiRequest.SendWebRequest();

            var recievedData = apiRequest.downloadHandler.data;
            if (recievedData.Length > 0)
            {
                url = Path.Combine(CachePath, assetBundle.Name);
                File.WriteAllBytes(url, recievedData);
            }
            else
            {
                yield break;
            }
        }
        private AssetBundle LoadFile(DataModels.AssetBundle model)
        {
            var url = Path.Combine(CachePath, model.Name);

            var assetBundle = AssetBundle.LoadFromFile(url);
            
            assetBundleInfos[model.Id].Loaded = assetBundle;

            return assetBundle;
            
        }
    }
}