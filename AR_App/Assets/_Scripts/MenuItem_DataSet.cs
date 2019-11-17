using DataModels;
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
    [SerializeField] Button infoButton;
    [SerializeField] Button loadButton;

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
        this.model = model;

        if (state == DataSetState.UpToDate || state == DataSetState.NoInternet)
        {
            downloadButton.interactable = false;
        }
        else if(state == DataSetState.ApiOnly)
        {
            loadButton.interactable = false;
        }
        else if(state == DataSetState.NeedUpdate)
        {
            downloadButton.GetComponent<Image>().sprite = updateIcon;
        }

        name_text.text = model.Name;

        downloadButton.onClick.AddListener(DownloadFile);
        loadButton.onClick.AddListener(LoadFile);
        // infoButton.onClick.AddListener(ShowInfo);
    }

    private void DownloadFile()
    {
        StartCoroutine(DataSetManager.Instance.FetchFile(
            model, 
            true, 
            (succes) => 
            {
                if (succes)
                {
                    downloadButton.interactable = false;
                }
            }));
    }

    private void LoadFile()
    {
        DataSetManager.Instance.OpenDataset(model, 
            (succes) =>
            {
                if (succes)
                {
                    loadButton.interactable = false;
                }
            });
    }

    private void ShowInfo()
    {
        
    }

}
