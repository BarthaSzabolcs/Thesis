using DataAcces;
using DataResources;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Vuforia;
using CustomConsole;
using AssetBundleManagment;

public class ContentHandler : MonoBehaviour, ITrackableEventHandler
{
    #region Events

    public delegate void TrackingChanged();

    public event TrackingChanged OnTrackingStart;
    public event TrackingChanged OnTrackingEnd;

    #endregion
    #region Hide in editor

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

                    ConsoleGUI.Instance.WriteLn($"Object found:\nId:{recognizedObject?.Id} Name: {recognizedObject?.Name}", Color.magenta);
                }
                else
                {
                    OnTrackingEnd?.Invoke();
                    ConsoleGUI.Instance.WriteLn($"Object lost:\nId:{recognizedObject?.Id} Name: {recognizedObject?.Name}", Color.magenta);
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
                if (recognizedObject.Content?.Dll != null)
                {
                    yield return DllManager.Instance.DownloadFile(recognizedObject.Content.Dll);
                }

                yield return AssetBundleManager.Instance.UseBundle(recognizedObject.Content.AssetBundle, LoadContent);
            }
        }
    }

    private void LoadContent(AssetBundle assetBundle)
    {
        if (assetBundle != null)
        {
            var contentGO = assetBundle.LoadAsset(recognizedObject.Content.Name);
            Instantiate(contentGO, transform);
        }
        else
        {
            ConsoleGUI.Instance.WriteLn($"{recognizedObject.Name} failed to load AssetBundle.");
        }
    }

    private IEnumerator GetRecognizedObject(int id)
    {
        var repo = new RecognizedObjectRepository();

        if (ConnectionManager.Instance.ApiReachable)
        {
            var url = $"{ ConnectionManager.Instance.ApiUrl }/api/RecognizedObject/{ id }";

            var request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            var jsonResponse = request.downloadHandler.text;
            recognizedObject = JsonConvert.DeserializeObject<RecognizedObjectResource>(jsonResponse,
                new JsonSerializerSettings { PreserveReferencesHandling = PreserveReferencesHandling.Objects });

            repo.CacheRecognizedObject(recognizedObject);
        }
        else
        {
            recognizedObject = repo.GetRecognizedObject(id);
        }
    }
}
