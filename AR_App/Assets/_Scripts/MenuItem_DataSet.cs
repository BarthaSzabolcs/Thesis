using DataModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum DataSetState
{
    UpToDate,
    NeedUpdate,
    ApiOnly,
    NoInternet
}

public class MenuItem_DataSet : MonoBehaviour
{
    #region Show in editor

    [SerializeField] Button downloadButton;
    [SerializeField] TextMeshProUGUI name_text;

    [Header("Icons:")]
    [SerializeField] private Sprite downloadIcon;
    [SerializeField] private Sprite updateIcon;
    
    #endregion
    #region Hide in editor

    private DataSet model;

    #endregion

    public void Initialize(DataSet model, DataSetState state)
    {
        if (state == DataSetState.UpToDate || state == DataSetState.NoInternet)
        {
            downloadButton.interactable = false;
        }
        else if(state == DataSetState.NeedUpdate)
        {
            downloadButton.GetComponent<Image>().sprite = updateIcon;
        }

        name_text.text = model.Name;
    }

    public void Download()
    {
        // ToDo - Download DataSet
    }

    public void ShowInfo()
    {
        // ToDo - Show Info
    }

}
