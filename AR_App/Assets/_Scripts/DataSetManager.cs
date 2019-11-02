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

    [Header("Connection:")]
    [SerializeField] private ConnectionConfig con;
    [SerializeField] private GameObject trackablePrefab;

    #endregion
    #region Hide in editor

    public static DataSetManager Instance { get; private set; }

    private Dictionary<string, RecognizedObjectResource> objectInfos;
    private string CachePath => Path.Combine(Application.persistentDataPath, "DataSets");

    // Managment
    private List<DataModels.DataSet> dataSetModels;

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

            Directory.CreateDirectory(CachePath);

            VuforiaBehaviour vb = FindObjectOfType<VuforiaBehaviour>();
            // ToDo - Remove
            vb.StartEvent += () => StartCoroutine(Test());
        }
    }

    private IEnumerator Test()
    {
        yield return PossibleDataSets();
        yield return LoadDataSet(dataSetModels[testDataSetIdx]);
    }
    private IEnumerator PossibleDataSets()
    {
        var url = string.Format("{0}/Api/DataSet", con.server);
        var apiRequest = UnityWebRequest.Get(url);

        Debug.Log("Get all DataSets.\n" + url);
        yield return apiRequest.SendWebRequest();

        var jsonResponse = apiRequest.downloadHandler.text;
        dataSetModels = JsonConvert.DeserializeObject<List<DataModels.DataSet>>(jsonResponse);
    }

    private IEnumerator LoadDataSet(DataModels.DataSet dataSetModel)
    {
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();

        yield return CacheDataSetFiles(dataSetModel);

        var path = Path.Combine(CachePath, dataSetModel.Name + ".xml");
        DataSet dataSet = objectTracker.CreateDataSet();
        if (dataSet.Load(path, VuforiaUnity.StorageType.STORAGE_ABSOLUTE))
        {
            objectTracker.Stop();

            var trackables = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();

            foreach (TrackableBehaviour trackable in trackables)
            {
                if (trackable.name == "New Game Object")
                {
                    trackable.gameObject.name = trackable.TrackableName;
                    trackable.gameObject.AddComponent<DefaultTrackableEventHandler>();

                    var contentHandler = trackable.gameObject.AddComponent<ContentHandler>();
                    contentHandler.Con = con;
                    StartCoroutine(contentHandler.Initialize());
                }
            }

            Debug.Log("Load " + dataSetModel.Name + " succeded.");
        }
        else
        {
            Debug.Log("Load " + dataSetModel.Name + " failed.");
        }
    }
    private IEnumerator CacheDataSetFiles(DataModels.DataSet dataSetModel)
    {
        string url = Path.Combine(CachePath, dataSetModel.Name);

        if (File.Exists(url + ".xml") == false)
        {
            yield return CacheFile(dataSetModel, true);
        }
        if (File.Exists(url + ".dat") == false)
        {
            yield return CacheFile(dataSetModel, false);
        }
    }
    private IEnumerator CacheFile(DataModels.DataSet dataSetModel, bool isXml)
    {
        var url = string.Format("{0}/api/DataSet/{1}/File?isXml={2}", con.server, dataSetModel.Id, isXml); 

        var apiRequest = UnityWebRequest.Get(url);
        yield return apiRequest.SendWebRequest();

        if (apiRequest.downloadHandler.data.Length > 0)
        {
            var path = Path.Combine(CachePath, dataSetModel.Name + (isXml ? ".xml" : ".dat"));

            File.WriteAllBytes(path, apiRequest.downloadHandler.data);
            Debug.Log("File " + dataSetModel.Name + (isXml ? ".xml" : ".dat") + "is cached to:\n" + path);
        }
        else
        {
            Debug.Log("File " + dataSetModel.Name + " was not found on the server.");
        }
    }

    // Trash

    //// (Could browse the dataSet-s in the UI)
    //// ToDo - Load info-s for the dataSet from the API /// param = string dataSetName
    //private Dictionary<string, RecognizedObjectResource> GetRecognizedObjects()
    //{
    //    var uri = con.server + "/api/RecognizedObject/";
    //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(uri));

    //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //    StreamReader reader = new StreamReader(response.GetResponseStream());
    //    string jsonResponse = reader.ReadToEnd();

    //    return JsonConvert.DeserializeObject<List<RecognizedObjectResource>>(jsonResponse).
    //        ToDictionary(x => x.Name, x => x);
    //}


    //void LoadDataSet(DataModels.DataSet dataSetModel)
    //{
    //    ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
    //    string dataPathWithoutFileType = Path.Combine(Application.persistentDataPath, dataSetModel.Name);

    //    DataSet dataSet = objectTracker.CreateDataSet();

    //    if (dataSet.Load(dataPathWithoutFileType + ".xml", VuforiaUnity.StorageType.STORAGE_ABSOLUTE))
    //    {
    //        objectTracker.Stop();
    //        objectInfos = GetRecognizedObjects();

    //        IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();

    //        foreach (TrackableBehaviour tb in tbs)
    //        {
    //            if (tb.name == "New Game Object")
    //            {
    //                tb.gameObject.name = tb.TrackableName;
    //                if (trackablePrefab != null)
    //                {
    //                    GameObject augmentation = Instantiate(trackablePrefab);

    //                    augmentation.transform.parent = tb.gameObject.transform;
    //                    augmentation.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
    //                }
    //                InitTrackableInfo(tb.gameObject);

    //                tb.gameObject.AddComponent<DefaultTrackableEventHandler>();
    //            }
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Failed to load dataset.");
    //    }
    //}

    //private void InitTrackableInfo(GameObject trackableObject)
    //{
    //    var textGO = trackableObject.transform.GetChild(0).GetChild(0);
    //    var text = textGO.GetComponent<TextMeshProUGUI>();

    //    if (objectInfos.TryGetValue(trackableObject.name, out var recognizedObject))
    //    {
    //        text.text = recognizedObject.Name;
    //    }
    //}
}