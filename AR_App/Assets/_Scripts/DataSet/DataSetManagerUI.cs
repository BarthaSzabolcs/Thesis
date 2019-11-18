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
    #region Hide in editor

    [SerializeField] private List<DataSetInfo_View> views = new List<DataSetInfo_View>();

    #endregion

    public void OpenMenu(IEnumerable<DataSetInfo_Model> dataSetInfos)
    {
        modeText.text = ConnectionManager.Instance.ApiState == ApiState.Online ?
            "Online":
            "Offline";

        PopulateMenu(dataSetInfos);

        canvas.SetActive(true);
    }

    private void PopulateMenu(IEnumerable<DataSetInfo_Model> dataSetInfos)
    {
        ClearList();

        foreach (var info in dataSetInfos)
        {
            var instance = Instantiate(menuItemPrefab, panelTransform);
            instance.GetComponent<DataSetInfo_View>().Initialize(info);
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
