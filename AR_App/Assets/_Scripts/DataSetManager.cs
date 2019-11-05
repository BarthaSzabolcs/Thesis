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

public class DataSetManager : MonoBehaviour
{
    #region Show in editor

    [Header("Test:")]
    [SerializeField] private int testDataSetIdx;
    [SerializeField] private GameObject trackablePrefab;
    [SerializeField] private TMP_InputField dataSetID;

    #endregion
    #region Hide in editor

    public static DataSetManager Instance { get; private set; }

    private Dictionary<string, RecognizedObjectResource> objectInfos;
    private string CachePath => Path.Combine(Application.persistentDataPath, "DataSets");

    // Managment
    private List<DataModels.DataSet> dataSetModels;

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

            //VuforiaBehaviour vb = FindObjectOfType<VuforiaBehaviour>();
            //vb.StartEvent += () => 
        }
    }
    
    #endregion

    public void StartTest()
    {
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        yield return PossibleDataSets();

        if (int.TryParse(dataSetID.text, out int id) &&
            dataSetModels.Any(x => x.Id == id))
        {
            yield return LoadDataSet(dataSetModels[dataSetModels.FindIndex(x => x.Id == id)]);
        }
    }
    private IEnumerator PossibleDataSets()
    {
        var url = string.Format("{0}/Api/DataSet", ConnectionManager.Instance.Con);
        var apiRequest = UnityWebRequest.Get(url);

        UILog.Instance.WriteLn($"Reqeust DataSets:\nurl:{url}");
        yield return apiRequest.SendWebRequest();

        var jsonResponse = apiRequest.downloadHandler.text;
        dataSetModels = JsonConvert.DeserializeObject<List<DataModels.DataSet>>(jsonResponse);

        UILog.Instance.Write("DataSets on the server:\n");
        foreach (var dataSetModel in dataSetModels)
        {
            UILog.Instance.Write($"Id: {dataSetModel.Id}\tName: '{dataSetModel.Name}'\n", "");
        }
        UILog.Instance.Write("=======================\n\n", "");
    }

    private IEnumerator LoadDataSet(DataModels.DataSet dataSetModel)
    {
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

        yield return CacheDataSetFiles(dataSetModel);

        var path = Path.Combine(CachePath, dataSetModel.Name + ".xml");
        DataSet dataSet = objectTracker.CreateDataSet();

        if (dataSet.Load(path, VuforiaUnity.StorageType.STORAGE_ABSOLUTE))
        {
            UILog.Instance.WriteLn($"Loading DataSet { dataSetModel.Name } succeded.", Color.green);

            objectTracker.Stop();

            if (objectTracker.ActivateDataSet(dataSet) == false)
            {
                UILog.Instance.WriteLn($"Could not activate DataSet { dataSetModel.Name }.", Color.red);
            }
            else
            {
                UILog.Instance.WriteLn($"DataSet { dataSetModel.Name } activated.", Color.green);
            }

            objectTracker.Start();

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
        else
        {
            UILog.Instance.WriteLn($"Loading DataSet { dataSetModel.Name } failed.", Color.red);
        }
    }
    private IEnumerator CacheDataSetFiles(DataModels.DataSet dataSetModel)
    {
        string url = Path.Combine(CachePath, dataSetModel.Name);

        if (File.Exists(url + ".xml") == false)
        {
            yield return CacheFile(dataSetModel, true);
        }
        else
        {
            UILog.Instance.WriteLn($"{ url }.xml found on device.", Color.green);
        }
        if (File.Exists(url + ".dat") == false)
        {
            yield return CacheFile(dataSetModel, false);
        }
        else
        {
            UILog.Instance.WriteLn($"{ url }.dat found on device.", Color.green);
        }
    }
    private IEnumerator CacheFile(DataModels.DataSet dataSetModel, bool isXml)
    {
        var url = string.Format("{0}/api/DataSet/{1}/File?isXml={2}", ConnectionManager.Instance.Con, dataSetModel.Id, isXml);

        UILog.Instance.WriteLn($"Download File: {dataSetModel.Name + (isXml ? ".xml" : ".dat")}\nurl: { url }");
        var request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.downloadHandler.data.Length > 0)
        {
            var path = Path.Combine(CachePath, dataSetModel.Name + (isXml ? ".xml" : ".dat"));

            File.WriteAllBytes(path, request.downloadHandler.data);
            UILog.Instance.WriteLn($"File { dataSetModel.Name } { (isXml ? ".xml" : ".dat") } is cached to:\n{ path}", Color.green);
        }
        else
        {
            UILog.Instance.WriteLn($"Could not get { dataSetModel.Name } from the server.", Color.red);
        }
    }
}