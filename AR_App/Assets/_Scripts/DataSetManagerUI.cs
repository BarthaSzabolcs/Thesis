using DataModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DataSetManagerUI : MonoBehaviour
{
    #region Show in editor

    [SerializeField] private GameObject canvas;
    [SerializeField] private RectTransform panelTransform;
    [SerializeField] private GameObject menuItemPrefab;
    [SerializeField] private TextMeshProUGUI modeText;

    #endregion

    public void OfflineMenu(IEnumerable<DataSet> offline)
    {
        SpawnMenuItems(offline, DataSetState.NoInternet);

        modeText.text = "Offline mode";

        canvas.SetActive(true);
    }
    public void OnlineMenu(IEnumerable<DataSet> offline, IEnumerable<DataSet> online)
    {
        var upToDate = offline.Where(off => online.First(on => on.Id == off.Id)?.Modified == off.Modified);
        var needUpDate = offline.Where(off => online.First(on => on.Id == off.Id)?.Modified == off.Modified);
        var apiOnly = online.Where(on => offline.Any(off => off.Id == on.Id) == false);

        SpawnMenuItems(upToDate, DataSetState.UpToDate);
        SpawnMenuItems(needUpDate, DataSetState.NeedUpdate);
        SpawnMenuItems(apiOnly, DataSetState.ApiOnly);

        modeText.text = "Online mode";

        canvas.SetActive(true);
    }

    private void SpawnMenuItems(IEnumerable<DataSet> models, DataSetState state)
    {
        foreach (var model in models)
        {
            var instance = Instantiate(menuItemPrefab, panelTransform);
            instance.GetComponent<MenuItem_DataSet>().Initialize(model, state);
        }
    }

}
