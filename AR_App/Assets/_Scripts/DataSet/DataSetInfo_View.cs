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

    private DataSetInfo_Model model;
    public DataSetInfo_Model Model
    {
        get => model;
        set
        {
            if (model != null)
            {
                model.PropertyChanged -= HandlePropertyChanged;
            }
            if (value != null)
            {
                value.PropertyChanged += HandlePropertyChanged;
            }
            model = value;
        }
    }

    #endregion

    #region Unity Callbacks

    public void OnDestroy()
    {
        model.PropertyChanged -= HandlePropertyChanged;
    }

    #endregion

    public void Initialize(DataSetInfo_Model info)
    {
        Model = info;

        var dataSet = info.Api ?? info.Cache;

        name_text.text = dataSet.Name;

        loadButton.interactable = info.Loaded is null && info.Cache != null;

        if (ConnectionManager.Instance.ApiReachable)
        {
            if (info.Cache != null)
            {
                if (info.Syncronised == false)
                {
                    downloadButton.GetComponent<Image>().sprite = updateIcon;
                }
                else
                {
                    downloadButton.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            downloadButton.gameObject.SetActive(false);
        }

        downloadButton.onClick.AddListener(DownloadFile);
        loadButton.onClick.AddListener(LoadFile);
    }

    private void DownloadFile()
    {
        StartCoroutine(DataSetManager.Instance.OpenFile(model.Api, true));
    }

    private void LoadFile()
    {
        DataSetManager.Instance.OpenDataset(model.Prefered);
    }

    private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(DataSetInfo_Model.Loaded):
                loadButton.interactable = model.Loaded is null;
                break;

            case nameof(DataSetInfo_Model.Api):
                break;

            case nameof(DataSetInfo_Model.Cache):
                if (model.Syncronised)
                {
                    downloadButton.GetComponent<Image>().sprite = downloadIcon;
                    downloadButton.interactable = false;
                }
                break;
        }
    }
}