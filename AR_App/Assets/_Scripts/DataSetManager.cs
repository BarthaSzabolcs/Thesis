using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;
using DataResources;
using DataAcces;
using System.Data.SqlClient;
using Mono.Data.Sqlite;
using System;

public enum ApiDataAcces { Offline, Online }

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

    // private Dictionary<string, RecognizedObjectResource> objectInfos;
    private List<DataModels.DataSet> avaliableSets;
    private List<int> loadedSetIds = new List<int>();
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

    private IEnumerator FetchPossibleDataSets()
    {
        yield return ConnectionManager.Instance.TestApiAcces();

        if (ConnectionManager.Instance.ApiDataAccesMode == ApiDataAcces.Online)
        {
            yield return FetchDataSetsOnline();
            managerUI.OnlineMenu(avaliableSets, repository.GetDataSets());
        }
        else
        {
            FetchDataSetsOffline();
            managerUI.OfflineMenu(repository.GetDataSets());
        }

        ConsoleGUI.Instance.Write("DataSets:\n");
        foreach (var dataSetModel in avaliableSets)
        {
            ConsoleGUI.Instance.Write($"Id: {dataSetModel.Id}\tName: '{dataSetModel.Name}'\n", "");
        }
        ConsoleGUI.Instance.Write("=======================\n\n", "");
    }

    public IEnumerator FetchFile(DataModels.DataSet dataSet, bool ignoreCache = false, Action<bool> callback = null)
    {
        string url = Path.Combine(CachePath, dataSet.Name);
        bool present = false;

        if (ConnectionManager.Instance.ApiDataAccesMode == ApiDataAcces.Offline)
        {
            ConsoleGUI.Instance.WriteLn($"Loading DataSet { dataSet.Name } from cache.");
            present = LoadFile(dataSet);
        }
        else if (ConnectionManager.Instance.ApiDataAccesMode == ApiDataAcces.Online)
        {
            ConsoleGUI.Instance.WriteLn($"Loading DataSet { dataSet.Name } from the API.");

            if (File.Exists(url + ".xml") == false || ignoreCache)
            {
                yield return DownloadFile(dataSet, true);
            }
            if (File.Exists(url + ".dat") == false || ignoreCache)
            {
                yield return DownloadFile(dataSet, false);
            }

            present = LoadFile(dataSet);
        }

        if (present)
        {
            InitializeTrackables(dataSet);
        }

        callback?.Invoke(present);
    }
    private IEnumerator FetchDataSetsOnline()
    {
        var url = $"{ ConnectionManager.Instance.ApiUrl }/Api/DataSet";
        ConsoleGUI.Instance.WriteLn($"Reqeust DataSets online:\nurl:{url}");

        var apiRequest = UnityWebRequest.Get(url);

        yield return apiRequest.SendWebRequest();
        ConsoleGUI.Instance.WriteLn($"Response arrived.");

        var jsonResponse = apiRequest.downloadHandler.text;
        avaliableSets = JsonConvert.DeserializeObject<List<DataModels.DataSet>>(jsonResponse);
    }
    private void FetchDataSetsOffline()
    {
        avaliableSets = repository.GetDataSets().ToList();
        ConsoleGUI.Instance.WriteLn($"DataSets loaded from cache.");
    }

    private IEnumerator DownloadFile(DataModels.DataSet dataSet, bool isXml)
    {
        var url = $"{ ConnectionManager.Instance.ApiUrl }/api/DataSet/{ dataSet.Id }/File?isXml={ isXml }";

        ConsoleGUI.Instance.WriteLn($"Download File: { dataSet.Name + (isXml ? ".xml" : ".dat") }\nurl: { url }");
        var request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.downloadHandler.data.Length > 0)
        {
            var path = Path.Combine(CachePath, dataSet.Name + (isXml ? ".xml" : ".dat"));

            File.WriteAllBytes(path, request.downloadHandler.data);

            repository.CacheDataSet(dataSet);
            ConsoleGUI.Instance.WriteLn($"File { dataSet.Name } { (isXml ? ".xml" : ".dat") } is cached to:\n{ path }", Color.green);
        }
        else
        {
            ConsoleGUI.Instance.WriteLn($"Could not find { dataSet.Name } from the server.", Color.red);
        }
    }
    private bool LoadFile(DataModels.DataSet dataSet)
    {
        var result = false;

        if (loadedSetIds.Contains(dataSet.Id))
            return result;

        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        DataSet loadedDataSet = objectTracker.CreateDataSet();

        var path = Path.Combine(CachePath, dataSet.Name + ".xml");

        if (loadedDataSet.Load(path, VuforiaUnity.StorageType.STORAGE_ABSOLUTE))
        {
            ConsoleGUI.Instance.WriteLn($"Loading DataSet { dataSet.Name } succeded.", Color.green);
            objectTracker.Stop();

            if (objectTracker.ActivateDataSet(loadedDataSet) == false)
            {
                ConsoleGUI.Instance.WriteLn($"Could not activate DataSet { dataSet.Name }.", Color.red);
            }
            else
            {
                ConsoleGUI.Instance.WriteLn($"DataSet { dataSet.Name } activated.", Color.green);
                loadedSetIds.Add(dataSet.Id);
                result = true;
            }

            objectTracker.Start();
        }
        else
        {
            ConsoleGUI.Instance.WriteLn($"Loading DataSet { dataSet.Name } failed.", Color.red);
        }

        return result;
    }
    private void InitializeTrackables(DataModels.DataSet dataSet)
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

    #region Button Actions

    public void OpenDataset(DataModels.DataSet model, Action<bool> callback = null)
    {
        var succes = LoadFile(model);

        if (succes)
        {
            InitializeTrackables(model);
        }

        callback?.Invoke(succes);
    }

    public void OpenDataSetMenu()
    {
        StartCoroutine(FetchPossibleDataSets());
    }

    #endregion
}
