using DataModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DataSetManagment
{
    public class DataSetManagerUI : MonoBehaviour
    {
        #region Show in editor

        [Header("Components")]
        [SerializeField] private GameObject canvas;
        [SerializeField] private RectTransform panelTransform;
        [SerializeField] private GameObject menuItemPrefab;
        [SerializeField] private TextMeshProUGUI modeText;

        #endregion
        #region Hide in editor

        [SerializeField] private List<DataSetInfo_View> views = new List<DataSetInfo_View>();

        #endregion

        public void OpenMenu(IEnumerable<DataSetInfo_Model> items)
        {
            modeText.text = ConnectionManager.Instance.ApiReachable ? "Online Mode" : "Offline Mode";

            PopulateMenu(items);

            canvas.SetActive(true);
        }

        private void PopulateMenu(IEnumerable<DataSetInfo_Model> models)
        {
            ClearList();

            foreach (var model in models)
            {
                var instance = Instantiate(menuItemPrefab, panelTransform);
                var view = instance.GetComponent<DataSetInfo_View>();
                view.Model = model;
                view.canvasTransform = canvas.transform.parent;
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
}