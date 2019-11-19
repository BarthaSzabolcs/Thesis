using DataModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DataSetManagment
{
    public class DataSetInfoDetails_View : MonoBehaviour
    {
        #region Show in editor

        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image image;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button switchButton;
        [SerializeField] private TextMeshProUGUI switchButtonText;

        #endregion
        #region Hide in editor

        private bool showOfflineVersion;
        private DataSetInfo_Model model;

        #endregion

        public void Initialize(DataSetInfo_Model model)
        {
            SyncData(model.Prefered);

            if (model.Cache != null && model.Api != null && model.Syncronised == false)
            {
                switchButtonText.text = "Show online version";
                switchButton.gameObject.SetActive(true);
                this.model = model;

                switchButton.onClick.AddListener(SwitchButtonClick);
            }

            closeButton.onClick.AddListener(() => Destroy(gameObject));
        }

        private void SyncData(DataSet model)
        {
            nameText.text = model.Name;
            descriptionText.text = model.Description;

            var size = descriptionText.GetPreferredValues();
            var textRect = descriptionText.GetComponent<RectTransform>();

            textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            //textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);

            // image.sprite = model.Image;
        }

        private void SwitchButtonClick()
        {
            if (showOfflineVersion)
            {
                SyncData(model.Cache);
                showOfflineVersion = false;
                switchButtonText.text = "Show online version";
            }
            else
            {
                SyncData(model.Api);
                showOfflineVersion = true;
                switchButtonText.text = "Show offline version";
            }
        }
    }
}