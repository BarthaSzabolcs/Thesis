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

public enum AccesMode { Offline, Online }

public class DataSetManager : MonoBehaviour
{
    #region Show in editor

    [Header("Test:")]
    [SerializeField] private GameObject trackablePrefab;
    [SerializeField] private TMP_InputField dataSetID;

    #endregion
    #region Hide in editor

    public static DataSetManager Instance { get; private set; }
    private string CachePath => Path.Combine(Application.persistentDataPath, "DataSets");

    private Dictionary<string, RecognizedObjectResource> objectInfos;
    private List<DataModels.DataSet> dataSetModels;
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

        StartCoroutine(GetPossibleDataSets());
    }

    #endregion

    public void Test()
    {
        if (int.TryParse(dataSetID.text, out int id))
        {
            var dataSet = dataSetModels.Find(x => x.Id == id);

            if (dataSet != null)
            {
                StartCoroutine(FetchFile(dataSet));
            }
        }
    }

    private IEnumerator GetPossibleDataSets()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ConnectionManager.Instance.Mode = AccesMode.Offline;
            FetchDataSetsOffline();
        }
        else
        {
            ConnectionManager.Instance.Mode = AccesMode.Online;
            yield return FetchDataSetsOnline();
        }

        UILog.Instance.Write("DataSets:\n");
        foreach (var dataSetModel in dataSetModels)
        {
            UILog.Instance.Write($"Id: {dataSetModel.Id}\tName: '{dataSetModel.Name}'\n", "");
        }
        UILog.Instance.Write("=======================\n\n", "");
    }
    
    private IEnumerator FetchFile(DataModels.DataSet dataSet)
    {
        string url = Path.Combine(CachePath, dataSet.Name);
        bool present = false;

        if (ConnectionManager.Instance.Mode == AccesMode.Offline)
        {
            UILog.Instance.WriteLn($"Loading DataSet { dataSet.Name } from cache.");
            present = LoadFile(dataSet);
        }
        else if (ConnectionManager.Instance.Mode == AccesMode.Online)
        {
            UILog.Instance.WriteLn($"Loading DataSet { dataSet.Name } from the API.");

            if (File.Exists(url + ".xml") == false)
            {
                yield return DownloadFile(dataSet, true);
            }
            if (File.Exists(url + ".dat") == false)
            {
                yield return DownloadFile(dataSet, false);
            }

            present = LoadFile(dataSet);
        }

        if (present)
        {
            InitializeTrackables(dataSet);
        }
    }
    private IEnumerator FetchDataSetsOnline()
    {
        var url = $"{ ConnectionManager.Instance.ApiUrl }/Api/DataSet";
        UILog.Instance.WriteLn($"Reqeust DataSets online:\nurl:{url}");

        var apiRequest = UnityWebRequest.Get(url);

        yield return apiRequest.SendWebRequest();
        UILog.Instance.WriteLn($"Response arrived.");

        var jsonResponse = apiRequest.downloadHandler.text;
        dataSetModels = JsonConvert.DeserializeObject<List<DataModels.DataSet>>(jsonResponse);
    }
    private void FetchDataSetsOffline()
    {
        dataSetModels = repository.GetDataSets().ToList();
        UILog.Instance.WriteLn($"DataSets loaded from cache.");
    }

    private IEnumerator DownloadFile(DataModels.DataSet dataSet, bool isXml)
    {
        var url = $"{ ConnectionManager.Instance.ApiUrl }/api/DataSet/{ dataSet.Id }/File?isXml={ isXml }";

        UILog.Instance.WriteLn($"Download File: { dataSet.Name + (isXml ? ".xml" : ".dat") }\nurl: { url }");
        var request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.downloadHandler.data.Length > 0)
        {
            var path = Path.Combine(CachePath, dataSet.Name + (isXml ? ".xml" : ".dat"));

            File.WriteAllBytes(path, request.downloadHandler.data);

            repository.CacheDataSet(dataSet);
            UILog.Instance.WriteLn($"File { dataSet.Name } { (isXml ? ".xml" : ".dat") } is cached to:\n{ path }", Color.green);
        }
        else
        {
            UILog.Instance.WriteLn($"Could not find { dataSet.Name } from the server.", Color.red);
        }
    }
    private bool LoadFile(DataModels.DataSet dataSet)
    {
        var result = false;

        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        DataSet loadedDataSet = objectTracker.CreateDataSet();

        var path = Path.Combine(CachePath, dataSet.Name + ".xml");

        if (loadedDataSet.Load(path, VuforiaUnity.StorageType.STORAGE_ABSOLUTE))
        {
            UILog.Instance.WriteLn($"Loading DataSet { dataSet.Name } succeded.", Color.green);
            objectTracker.Stop();

            if (objectTracker.ActivateDataSet(loadedDataSet) == false)
            {
                UILog.Instance.WriteLn($"Could not activate DataSet { dataSet.Name }.", Color.red);
            }
            else
            {
                UILog.Instance.WriteLn($"DataSet { dataSet.Name } activated.", Color.green);
                result = true;
            }

            objectTracker.Start();
        }
        else
        {
            UILog.Instance.WriteLn($"Loading DataSet { dataSet.Name } failed.", Color.red);
        }

        return result;
    }
    private void InitializeTrackables(DataModels.DataSet dataSet)
    {
        var trackables = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();

        foreach (TrackableBehaviour trackable in trackables)
        {
            UILog.Instance.WriteLn($"Loading Trackable '{ trackable.TrackableName }' succeded.");

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

    //private IEnumerator CacheDataSetFiles(DataModels.DataSet dataSetModel)
    //{
    //    string url = Path.Combine(CachePath, dataSetModel.Name);

    //    //ToDo - Offline version
    //    if (File.Exists(url + ".xml") == false)
    //    {
    //        yield return DownloadDataSetFile(dataSetModel, true);
    //    }
    //    else
    //    {
    //        UILog.Instance.WriteLn($"{ url }.xml found on device.", Color.green);
    //    }
    //    //ToDo - Offline version
    //    if (File.Exists(url + ".dat") == false)
    //    {
    //        yield return DownloadDataSetFile(dataSetModel, false);
    //    }
    //    else
    //    {
    //        UILog.Instance.WriteLn($"{ url }.dat found on device.", Color.green);
    //    }
    //}