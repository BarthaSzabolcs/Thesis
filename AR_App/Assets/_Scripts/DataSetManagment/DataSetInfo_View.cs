using DataModels;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DataSetManagment
{
    public class DataSetInfo_View : MonoBehaviour
    {
        #region Show in editor

        [SerializeField] private Button downloadButton;
        [SerializeField] private Button infoButton;
        [SerializeField] private Button loadButton;

        [SerializeField] private TextMeshProUGUI name_text;
        [SerializeField] private GameObject detailsWindow;

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

                SyncWithModel();
            }
        }

        public Transform canvasTransform { get; set; }

        #endregion

        #region Unity Callbacks

        public void OnDestroy()
        {
            model.PropertyChanged -= HandlePropertyChanged;
        }

        #endregion

        public void SyncWithModel()
        {
            var dataSet = Model.Api ?? Model.Cache;

            name_text.text = dataSet.Name;

            loadButton.interactable = Model.Loaded is null && Model.Cache != null;

            if (ConnectionManager.Instance.ApiReachable)
            {
                if (Model.Cache != null)
                {
                    if (Model.Syncronised == false)
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

            downloadButton.onClick.RemoveAllListeners();
            downloadButton.onClick.AddListener(DownloadFile);

            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(LoadFile);

            infoButton.onClick.RemoveAllListeners();
            infoButton.onClick.AddListener(ShowInfo);
        }

        private void DownloadFile()
        {
            StartCoroutine(DataSetManager.Instance.OpenFile(model.Api, true));
        }

        private void LoadFile()
        {
            DataSetManager.Instance.OpenDataset(model.Prefered);
        }

        private void ShowInfo()
        {
            var details = Instantiate(detailsWindow, canvasTransform).GetComponent<DataSetInfoDetails_View>();

            details.Initialize(Model);
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
                        downloadButton.gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }
}