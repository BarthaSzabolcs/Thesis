using DataResources;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;

public class ContentHandler : MonoBehaviour, ITrackableEventHandler
{
    #region Events

    public delegate void TrackingChanged();

    public TrackingChanged OnTrackingStart;
    public TrackingChanged OnTrackingEnd;

    #endregion
    #region Hide in editor

    public ConnectionConfig Con { get; set; }

    private bool visible;
    private bool Visible
    {
        get => visible;
        set
        {
            if (visible != value)
            {
                if (value)
                {
                    OnTrackingStart?.Invoke();
                }
                else
                {
                    OnTrackingEnd?.Invoke();
                }
            }
            visible = value;
        }
    }

    private RecognizedObjectResource recognizedObject;

    #endregion

    public void Awake()
    {
        var mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus, TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {
            Visible = true;
        }
        else
        {
            Visible = false;
        }
    }

    public IEnumerator Initialize()
    {
        if (int.TryParse(gameObject.name, out var id))
        {
            yield return GetRecognizedObject(id);

            if (recognizedObject != null)
            {
                yield return AssetBundleManager.Instance.Load(recognizedObject.Content.AssetBundle);

                var assetBundle = AssetBundleManager.Instance.Loaded[recognizedObject.Content.AssetBundle.Name];
                var contentGO = assetBundle.LoadAsset(recognizedObject.Content.Name);

                Instantiate(contentGO, transform);
            }
            else
            {
                Debug.Log("Could not found recognized object with an id of " + id);
            }
        }
        else
        {
            Debug.Log("Could not parse the name of the trackable as an int.");
        }
    }

    private IEnumerator GetRecognizedObject(int id)
    {
        var url = string.Format("{0}/api/RecognizedObject/{1}", Con.server, id);

        var request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        var jsonResponse = request.downloadHandler.text;
        recognizedObject = JsonConvert.DeserializeObject<RecognizedObjectResource>(jsonResponse);
    }
}
