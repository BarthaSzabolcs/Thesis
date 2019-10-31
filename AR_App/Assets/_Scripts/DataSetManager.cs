using Newtonsoft.Json;
using DataAcces.Resources;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;

public class DataSetManager : MonoBehaviour
{
    #region Show in editor

    [Header("Connection:")]
    [SerializeField] private ConnectionConfig connection;

    [SerializeField] private GameObject trackablePrefab;

    #endregion
    #region Hide in editor

    public static DataSetManager Instance { get; private set; }

    private Dictionary<string, RecognizedObjectResource> objectInfos;
    private string CachePath => Path.Combine(Application.persistentDataPath, "DataSets");

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
            Directory.CreateDirectory(CachePath);

            DontDestroyOnLoad(gameObject);

            // VuforiaBehaviour vb = FindObjectOfType<VuforiaBehaviour>();
            // vb.StartEvent += LoadDataSet;
        }
    }

    void LoadDataSet(FileInfo info)
    {
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        string dataPathWithoutFileType = Path.Combine(Application.persistentDataPath, info.Name);

        DataSet dataSet = objectTracker.CreateDataSet();

        if (dataSet.Load(dataPathWithoutFileType + ".xml", VuforiaUnity.StorageType.STORAGE_ABSOLUTE))
        {
            objectTracker.Stop();
            objectInfos = GetDataSetInfo();

            IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();

            foreach (TrackableBehaviour tb in tbs)
            {
                if (tb.name == "New Game Object")
                {
                    tb.gameObject.name = tb.TrackableName;
                    if (trackablePrefab != null)
                    {
                        GameObject augmentation = Instantiate(trackablePrefab);

                        augmentation.transform.parent = tb.gameObject.transform;
                        augmentation.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                    }
                    InitTrackableInfo(tb.gameObject);

                    tb.gameObject.AddComponent<DefaultTrackableEventHandler>();
                }
            }
        }
        else
        {
            Debug.Log("Failed to load dataset.");
        }
    }

    private IEnumerator CacheFile(DataAcces.DataModels.FileInfo info)
    {
        string url = Path.Combine(CachePath, info.Name + ".xml");

        if (File.Exists(url) == false)
        {
            url = connection.server + "/Api/File/" + info.Id;

            var apiRequest = UnityWebRequest.Get(url);
            yield return apiRequest.SendWebRequest();

            url = Path.Combine(CachePath, info.Name);

            File.WriteAllBytes(url, apiRequest.downloadHandler.data);
        }
    }

    // (Could browse the dataSet-s in the UI)
    // ToDo - Load info-s for the dataSet from the API /// param = string dataSetName
    private Dictionary<string, RecognizedObjectResource> GetDataSetInfo()
    {
        var uri = connection.server + "/api/RecognizedObject/";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(uri));

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();

        return JsonConvert.DeserializeObject<List<RecognizedObjectResource>>(jsonResponse).
            ToDictionary(x => x.Name, x => x);
    }

    private void InitTrackableInfo(GameObject trackableObject)
    {
        var textGO = trackableObject.transform.GetChild(0).GetChild(0);
        var text = textGO.GetComponent<TextMeshProUGUI>();

        if (objectInfos.TryGetValue(trackableObject.name, out var recognizedObject))
        {
            text.text = recognizedObject.Name;
        }
    }

}