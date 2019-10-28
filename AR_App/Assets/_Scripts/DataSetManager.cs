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

public class DataSetManager : MonoBehaviour
{
    #region Show in editor

    [Header("Connection:")]
    [SerializeField] private ConnectionConfig con;

    [Header("Dunno:")]
    [SerializeField] private string fileName;
    [SerializeField] private GameObject trackableObject;

    #endregion
    #region Hide in editor

    private Dictionary<string, RecognizedObject> objectInfos;
    
    #endregion

    void Start()
    {
        VuforiaBehaviour vb = FindObjectOfType<VuforiaBehaviour>();
        // vb.StartEvent += LoadDataSet;

        // StartCoroutine(InstantiateObject());
    }

    void LoadDataSet()
    {
        ObjectTracker objectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
        string dataPath = Path.Combine(/*Application.persistentDataPath,*/ fileName + ".xml");

        DataSet dataSet = objectTracker.CreateDataSet();

        if (dataSet.Load(dataPath, VuforiaUnity.StorageType.STORAGE_ABSOLUTE))
        {
            objectTracker.Stop();
            objectInfos = GetDataSetInfo();

            IEnumerable<TrackableBehaviour> tbs = TrackerManager.Instance.GetStateManager().GetTrackableBehaviours();

            foreach (TrackableBehaviour tb in tbs)
            {
                if (tb.name == "New Game Object")
                {
                    tb.gameObject.name = tb.TrackableName;
                    if (trackableObject != null)
                    {
                        GameObject augmentation = Instantiate(trackableObject);

                        augmentation.transform.parent = tb.gameObject.transform;
                        augmentation.transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
                    }
                    InitTrackableInfo(tb.gameObject);

                    tb.gameObject.AddComponent<DefaultTrackableEventHandler>();
                    // tb.gameObject.AddComponent<TurnOffBehaviour>();
                }
            }
        }
        else
        {
            Debug.Log("Failed to load dataset.");
        }
    }

    // (Could browse the dataSet-s in the UI)
    // ToDo - Load info-s for the dataSet from the API /// param = string dataSetName
    private Dictionary<string, RecognizedObject> GetDataSetInfo()
    {
        var uri = con.server + "/api/RecognizedObject/";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(uri));

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();

        return JsonConvert.DeserializeObject<List<RecognizedObject>>(jsonResponse).
            ToDictionary(x => x.Name, x => x);
    }

    private void InitTrackableInfo(GameObject trackableObject)
    {
        var textGO = trackableObject.transform.GetChild(0).GetChild(0);
        var text = textGO.GetComponent<TextMeshProUGUI>();

        if (objectInfos.TryGetValue(trackableObject.name, out var recognizedObject))
        {
            text.text = recognizedObject.Description;
        }
    }
}