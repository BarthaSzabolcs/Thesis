using CustomConsole;
using DataAcces;
using DataModels;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace DllManagment
{
    public class DllManager : MonoBehaviour
    {
        #region Hide in editor

        public static DllManager Instance { get; private set; }

        private string CachePath
        {
            get
            {
                return Path.Combine(Application.persistentDataPath, "Dll");
            }
        }
        private DllRepository repo;
        private Dictionary<int, DllInfo> dllInfos = new Dictionary<int, DllInfo>();

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
                repo = new DllRepository();
            }
        }
        private void Start()
        {
            StartCoroutine(FetchDlls());
        }

        #endregion

        private IEnumerator FetchDlls()
        {
            yield return ConnectionManager.Instance.TestApiAcces();

            if (ConnectionManager.Instance.ApiReachable)
            {
                yield return FetchDllsFromApi();
            }

            FetchDllsFromCache();
        }
        private void FetchDllsFromCache()
        {
            var cache = repo.GetAll();

            foreach (var dll in cache)
            {
                if (dllInfos.TryGetValue(dll.Id, out var info))
                {
                    info.Cache = dll;
                }
                else
                {
                    dllInfos.Add(dll.Id, new DllInfo { Cache = dll });
                }
            }
        }
        private IEnumerator FetchDllsFromApi()
        {
            var url = $"{ ConnectionManager.Instance.ApiUrl }/Api/Dll";
            var apiRequest = UnityWebRequest.Get(url);

            yield return apiRequest.SendWebRequest();

            var jsonResponse = apiRequest.downloadHandler.text;
            var onlineDlls = JsonConvert.DeserializeObject<List<Dll>>(jsonResponse,
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

            foreach (var dll in onlineDlls)
            {
                if (dllInfos.TryGetValue(dll.Id, out var info))
                {
                    info.Api = dll;
                }
                else
                {
                    dllInfos.Add(dll.Id, new DllInfo { Api = dll });
                }
            }
        }

        public IEnumerator Load(Dll model)
        {
            if (dllInfos.TryGetValue(model.Id, out var info))
            {
                if (ConnectionManager.Instance.ApiReachable && info.Syncronised == false)
                {
                    yield return DownloadFile(info.Api);
                }

                LoadFile(info.Prefered);
            }
            else
            {
                ConsoleGUI.Instance.WriteLn($"Could not found DllInfo with an id of { model?.Id }.", Color.red);
            }
        }
        public IEnumerator DownloadFile(Dll model)
        {
            string url = $"{ConnectionManager.Instance.ApiUrl}/Api/Dll/{ model.Id }/File";

            var apiRequest = UnityWebRequest.Get(url);
            yield return apiRequest.SendWebRequest();

            var recievedData = apiRequest.downloadHandler.data;
            if (recievedData.Length > 0)
            {
                url = Path.Combine(CachePath, $"{model.Name}.dll");
                File.WriteAllBytes(url, recievedData);

                ConsoleGUI.Instance.WriteLn($"Downloading of assembly({ model.Name }) succesful.", Color.green);
            }
            else
            {
                yield break;
            }
        }
        private void LoadFile(Dll model)
        {
            if (dllInfos[model.Id].assembly is null)
            {
                var name = AssemblyName.GetAssemblyName(Path.Combine(CachePath, $"{ model.Name }.dll"));
                dllInfos[model.Id].assembly = Assembly.Load(name);

                if (dllInfos[model.Id].assembly != null)
                {
                    ConsoleGUI.Instance.WriteLn($"Loading of assembly({ model.Name }) succesful.", Color.green);
                }
                else
                {
                    ConsoleGUI.Instance.WriteLn($"Loading of assembly({ model.Name }) failed.", Color.red);
                }
            }
            else
            {
                ConsoleGUI.Instance.WriteLn($"loading of assembly({ model.Name }) skipped, already loaded.", Color.magenta);
            }
        }
    }
}