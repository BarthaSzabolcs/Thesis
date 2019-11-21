using CustomConsole;
using DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

public class DllManager : MonoBehaviour
{
    #region Hide in editor

    public string CachePath => Path.Combine(Application.persistentDataPath, "Dll");
    public static DllManager Instance { get; private set; }

    #endregion

    void Awake()
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

    private void LoadDll(Dll model)
    {
        var assembly = Assembly.LoadFile(Path.Combine(CachePath, $"{model.Name}.dll"));

        if (assembly != null)
        {
            ConsoleGUI.Instance.WriteLn($"Loading of assembly({ assembly.FullName }) succesful.", Color.magenta);
        }
        else
        {
            ConsoleGUI.Instance.WriteLn($"Loading of assembly({ assembly.FullName }) failed.", Color.red);
        }
    }

    public IEnumerator DownloadFile(Dll model, Action<Dll> callback = null)
    {
        string url = $"{ConnectionManager.Instance.ApiUrl}/Api/Dll/{ model.Id }/File";

        var apiRequest = UnityWebRequest.Get(url);
        yield return apiRequest.SendWebRequest();

        var recievedData = apiRequest.downloadHandler.data;
        if (recievedData.Length > 0)
        {
            url = Path.Combine(CachePath, $"{model.Name}.dll");
            File.WriteAllBytes(url, recievedData);
        }
        else
        {
            yield break;
        }

        LoadDll(model);

        //callback?.Invoke(model);
    }
}