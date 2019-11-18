using DataModels;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataSetInfo_View : MonoBehaviour
{
    #region Show in editor

    [SerializeField] Button downloadButton;
    [SerializeField] Button infoButton;
    [SerializeField] Button loadButton;

    [SerializeField] TextMeshProUGUI name_text;

    [Header("Icons:")]
    [SerializeField] private Sprite downloadIcon;
    [SerializeField] private Sprite updateIcon;
    
    #endregion
    #region Hide in editor

    private DataSetInfo_Model info;
    public DataSetInfo_Model Info
    {
        get => info;
        set
        {
            if (info != null)
            {
                info.PropertyChanged -= HandlePropertyChanged;
            }
            if (value != null)
            {
                value.PropertyChanged += HandlePropertyChanged;
            }
            info = value;
        }
    }

    #endregion

    #region Unity Callbacks

    public void OnDestroy()
    {
        info.PropertyChanged -= HandlePropertyChanged;
    }

    #endregion

    public void Initialize(DataSetInfo_Model info)
    {
        Info = info;

        var dataSet = info.Api ?? info.Cache;

        name_text.text = dataSet.Name;

        loadButton.interactable = info.Loaded is null;

        if (info.Cache != null && info.Syncronised == false)
        {
            downloadButton.GetComponent<Image>().sprite = updateIcon;
        }
        else
        {
            downloadButton.GetComponent<Image>().sprite = downloadIcon;
            downloadButton.interactable = info.Cache is null;
        }

        downloadButton.onClick.AddListener(DownloadFile);
        loadButton.onClick.AddListener(LoadFile);
        // infoButton.onClick.AddListener(ShowInfo);
    }

    private void DownloadFile()
    {
        StartCoroutine(DataSetManager.Instance.OpenFile(info.Api, true));
    }

    private void LoadFile()
    {
        DataSetManager.Instance.OpenDataset(info.Prefered);
    }

    private void ShowInfo()
    {
        
    }

    private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(DataSetInfo_Model.Loaded):
                loadButton.interactable = info.Loaded is null;
                break;

            case nameof(DataSetInfo_Model.Api):
                break;

            case nameof(DataSetInfo_Model.Cache):
                if (info.Syncronised)
                {
                    downloadButton.GetComponent<Image>().sprite = downloadIcon;
                    downloadButton.interactable = false;
                }
                break;
        }
    }
}