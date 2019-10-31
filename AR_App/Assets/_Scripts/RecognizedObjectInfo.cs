using DataAcces.Resources;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine;

public class RecognizedObjectInfo : MonoBehaviour
{
    #region Show in editor

    [SerializeField] private ConnectionConfig con;
    [SerializeField] private float waitTime;

    #endregion
    #region Hide in editor

    private float timer;
    private bool triggered = false;
    
    #endregion

    private void Update()
    {
        try
        {
            if (Time.time - timer > waitTime && triggered == false)
            {
                triggered = true;
                StartCoroutine(Init());
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private RecognizedObjectResource GetObjectInfo(int id)
    {
        var uri = con.server + "/Api/RecognizedObject/" + id;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        
        return JsonConvert.DeserializeObject<RecognizedObjectResource>(jsonResponse);
    }

    private IEnumerator Init()
    {
        int id = int.Parse(gameObject.name);
        var info = GetObjectInfo(id);

        if (AssetBundleManager.Instance.Loaded.ContainsKey(info.Content.FileInfo.Name) == false)
        {
            yield return AssetBundleManager.Instance.Load(info.Content.FileInfo);
        }
        var assetBundle = AssetBundleManager.Instance.Loaded[info.Content.FileInfo.Name];

        var go = assetBundle.LoadAsset<GameObject>(info.Content.Name);
        Instantiate(go, Vector3.zero, Quaternion.identity, transform);
    }
}