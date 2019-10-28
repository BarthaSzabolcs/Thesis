using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine;

public class RecognizedObjectInfo : MonoBehaviour
{
    [SerializeField] private string uri;
    [SerializeField] private TextMeshProUGUI descriptionTMP;
    [SerializeField] private DefaultTrackableEventHandler trackableEventHandler;

    [SerializeField] private float waitTime;
    private int timer;

    private void Update()
    {
        try
        {
            if (Time.time - timer > waitTime)
            {
                int id = int.Parse(gameObject.transform.parent.name);

                var info = GetObjectInfo(id);

                if (info != null)
                {
                    descriptionTMP.text = string.Format("ID: {0}\nName: {1}\nDescription: {2}\n", 
                        info.Id, info.Name, info.Description);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            throw;
        }
    }

    private RecognizedObject GetObjectInfo(int id)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(uri, id));

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string jsonResponse = reader.ReadToEnd();
        RecognizedObject info = JsonUtility.FromJson<RecognizedObject>(jsonResponse);

        return info;
    }
}

public class RecognizedObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}