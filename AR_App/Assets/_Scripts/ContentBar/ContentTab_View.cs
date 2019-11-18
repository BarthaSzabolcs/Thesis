using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ContentBar
{
    public class ContentTab_View : MonoBehaviour
    {
        #region Show in editor

        [SerializeField] private float textMargin;

        [Header("Colors")]
        [SerializeField] private Color normalButtonColor;
        [SerializeField] private Color selectedButtonColor;

        [SerializeField] private Color normalTextColor;
        [SerializeField] private Color selectedTextColor;

        [Header("Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Image buttonImage;

        #endregion

        private ContentTab_Model model;
        public ContentTab_Model Model
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

        private void SyncWithModel()
        {
            HandleNameChange();
            HandleOpenChange();
        }

        private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ContentTab_Model.Name):
                    HandleNameChange();
                    break;

                case nameof(ContentTab_Model.Open):
                    HandleOpenChange();
                    break;
            }
        }

        private void HandleNameChange()
        {
            nameText.text = Model.Name;

            var correctWidth = nameText.preferredWidth + textMargin * 2;
            var buttonRectTransform = buttonImage.gameObject.GetComponent<RectTransform>();
            buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, correctWidth);
        }

        private void HandleOpenChange()
        {
            if (Model.Open)
            {
                nameText.color = selectedTextColor;
                buttonImage.color = selectedButtonColor;
            }
            else
            {
                nameText.color = normalTextColor;
                buttonImage.color = normalButtonColor;
            }
        }
    }
}
