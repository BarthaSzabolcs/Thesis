using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ContentBar
{
    public class ContentTabBar : MonoBehaviour
    {
        #region Show in editor

        [SerializeField] GameObject buttonPrefab;
        [SerializeField] RectTransform barTransform;

        #endregion
        #region Hide in editor

        public static ContentTabBar Instance { get; private set; }
        private List<ContentTab_View> tabs = new List<ContentTab_View>();

        private ContentTab_View activeTab;
        public ContentTab_View ActiveTab
        {
            get => activeTab;
            set
            {
                if (activeTab != null)
                {
                    activeTab.Model.CloseAction();
                    activeTab.Model.Open = false;
                }
                if (value != null)
                {
                    value.Model.OpenAction();
                    value.Model.Open = true;
                }

                activeTab = value;
            }
        }

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        #endregion

        public void AddTab(ContentTab_Model model)
        {
            var button = Instantiate(buttonPrefab, barTransform);
            var view = button.GetComponent<ContentTab_View>();

            view.Model = model;
            tabs.Add(view);

            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                ActiveTab = view;
            });
        }
    }
}
