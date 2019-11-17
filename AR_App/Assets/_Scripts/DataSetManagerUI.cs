using DataModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataSetManagerUI : MonoBehaviour
{
    #region Show in editor

    [SerializeField] private GameObject canvas;
    [SerializeField] private RectTransform panelTransform;
    [SerializeField] private GameObject menuItemPrefab;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private Button menuButton;

    #endregion

    public void OfflineMenu(IEnumerable<DataSet> offline)
    {
        ClearList();

        var dataSetsWithState = offline.Select(offlineSet => (offlineSet, DataSetState.NoInternet));

        SpawnMenuItems(dataSetsWithState);

        modeText.text = "Offline mode";

        canvas.SetActive(true);
    }
    public void OnlineMenu(IEnumerable<DataSet> online, IEnumerable<DataSet> offline)
    {
        ClearList();

        var modelsWithState = new Dictionary<int, (DataSet dataSet, DataSetState state)>();

        foreach (var onlineSet in online)
        {
            modelsWithState.Add(onlineSet.Id, (onlineSet, DataSetState.ApiOnly));
        }

        foreach (var offlineSet in offline)
        {
            if (modelsWithState.TryGetValue(offlineSet.Id, out var item))
            {
                if (item.dataSet.Modified == offlineSet.Modified)
                {
                    modelsWithState[offlineSet.Id] = (item.dataSet, DataSetState.UpToDate);
                }
                else
                {
                    modelsWithState[offlineSet.Id] = (item.dataSet, DataSetState.NeedUpdate);
                }
            }
        }

        modeText.text = "Online mode";

        SpawnMenuItems(modelsWithState.Values);

        canvas.SetActive(true);
    }

    private void SpawnMenuItems(IEnumerable<(DataSet dataSet, DataSetState state)> modelsWithState)
    {
        foreach (var modelWithState in modelsWithState)
        {
            var instance = Instantiate(menuItemPrefab, panelTransform);
            instance.GetComponent<MenuItem_DataSet>().Initialize(modelWithState.dataSet, modelWithState.state);
        }
    }

    private void ClearList()
    {
        for (int i = panelTransform.childCount - 1; i >= 0; i--)
        {
            Destroy(panelTransform.GetChild(i).gameObject);
        }
    }

}
