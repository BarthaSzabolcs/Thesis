using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;
using DataAcces;
using System;
using CustomConsole;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class DataSetManager : MonoBehaviour
{
    #region Show in editor

    [Header("Test:")]
    [SerializeField] private GameObject trackablePrefab;
    [SerializeField] private TMP_InputField dataSetID;
    [SerializeField] private DataSetManagerUI managerUI;

    #endregion
    #region Hide in editor

    public static DataSetManager Instance { get; private set; }
    private string CachePath => Path.Combine(Application.persistentDataPath, "DataSets");

    private Dictionary<int, DataSetInfo_Model> dataSetInfos = new Dictionary<int, DataSetInfo_Model>();
    private DataSetRepository repository;

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
    private void Start()
    {
        repository = new DataSetRepository();
    }

    #endregion

    #region Button Actions

    public void OpenDataset(DataModels.DataSet model, Action<bool> callback = null)
    {
        var success = LoadInFile(model);

        if (success)
        {
            InitFile(model);
        }

        callback?.Invoke(success);
    }
    public void OpenDataSetMenu()
    {
        StartCoroutine(FetchDataSets());
    }
    //public void CloseDataset(DataModels.DataSet model, Action<bool> callback = null)
    //{
    //    if (dataSetInfos.TryGetValue(model.Id, out var info) && info.Loaded != null)
    //    {
    //        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
    //        objectTracker.Stop();

    //        objectTracker.DeactivateDataSet(info.Loaded);
    //        objectTracker.DestroyDataSet(info.Loaded, false);
            

    //        info.Loaded = null;

    //        objectTracker.Start();
    //    }
    //}

    #endregion

    private IEnumerator FetchDataSets()
    {
        yield return ConnectionManager.Instance.TestApiAcces();

        if (ConnectionManager.Instance.ApiReachable)
        {
            yield return FetchDataSetsFromApi();
        }
        FetchDataSetsFromCache();

        managerUI.OpenMenu(dataSetInfos.Values);
    }
    private IEnumerator FetchDataSetsFromApi()
    {
        Debug.Log("Load datasets from API.");

        var url = $"{ ConnectionManager.Instance.ApiUrl }/Api/DataSet";
        var apiRequest = UnityWebRequest.Get(url);

        yield return apiRequest.SendWebRequest();

        var jsonResponse = apiRequest.downloadHandler.text;
        var onlineSets = JsonConvert.DeserializeObject<List<DataModels.DataSet>>(jsonResponse);

        foreach (var set in onlineSets)
        {
            if (dataSetInfos.TryGetValue(set.Id, out var info))
            {
                info.Api = set;
            }
            else
            {
                dataSetInfos.Add(set.Id, new DataSetInfo_Model { Api = set });
            }
        }
    }
    private void FetchDataSetsFromCache()
    {
        Debug.Log("Load datasets from device.");

        var cachedSets = repository.GetDataSets();

        foreach (var set in cachedSets)
        {
            if (dataSetInfos.TryGetValue(set.Id, out var info))
            {
                info.Cache = set;
            }
            else
            {
                dataSetInfos.Add(set.Id, new DataSetInfo_Model { Cache = set });
            }
        }
    }

    public IEnumerator OpenFile(DataModels.DataSet dataSet, bool updateCache = false, Action<bool> callback = null)
    {
        string url = Path.Combine(CachePath, dataSet.Name);

        if (ConnectionManager.Instance.ApiReachable)
        {
            Debug.Log($"Loading DataSet { dataSet.Name } from API.");

            if (File.Exists($"{url}.xml") == false || updateCache)
            {
                yield return DownloadFile(dataSet, true);
            }

            if (File.Exists($"{url}.dat") == false || updateCache)
            {
                yield return DownloadFile(dataSet, false);
            }
        }

        var success = LoadInFile(dataSet);

        if (success)
        {
            InitFile(dataSet);
        }

        callback?.Invoke(success);
    }
    private IEnumerator DownloadFile(DataModels.DataSet model, bool isXml)
    {
        var url = $"{ ConnectionManager.Instance.ApiUrl }/api/DataSet/{ model.Id }/File?isXml={ isXml }";
        var request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.downloadHandler.data.Length > 0)
        {
            var path = Path.Combine(CachePath, model.Name + (isXml ? ".xml" : ".dat"));

            File.WriteAllBytes(path, request.downloadHandler.data);

            repository.CacheDataSet(model);
        }
    }
    private bool LoadInFile(DataModels.DataSet model)
    {
        var success = false;

        if (dataSetInfos.TryGetValue(model.Id, out var info) && info.Loaded != null)
            return success;
        
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        info.Loaded = objectTracker.CreateDataSet();

        var path = Path.Combine(CachePath, model.Name + ".xml");

        if (info.Loaded.Load(path, VuforiaUnity.StorageType.STORAGE_ABSOLUTE))
        {
            objectTracker.Stop();

            if (objectTracker.ActivateDataSet(info.Loaded) != false)
            {
                Debug.Log($"DataSet { model.Name } activated.");
                success = true;
            }
            else
            {
                Debug.LogError($"Could not activate DataSet { model.Name }.");
            }

            objectTracker.Start();
        }
        else
        {
            Debug.LogError($"Loading DataSet { model.Name } failed.");
        }

        return success;
    }
    private void InitFile(DataModels.DataSet model)
    {
        var trackables = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();

        foreach (TrackableBehaviour trackable in trackables)
        {
            ConsoleGUI.Instance.WriteLn($"Loading Trackable '{ trackable.TrackableName }' succeded.");

            if (trackable.name == "New Game Object")
            {
                trackable.gameObject.name = trackable.TrackableName;
                trackable.gameObject.AddComponent<DefaultTrackableEventHandler>();

                var contentHandler = trackable.gameObject.AddComponent<ContentHandler>();
                StartCoroutine(contentHandler.Initialize());
            }
        }
    }

}