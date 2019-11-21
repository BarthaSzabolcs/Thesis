using ContentBar;
using CustomConsole;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace FormExample
{
    public class MaintanceForm : MonoBehaviour
    {
        #region Show in editor

        [SerializeField] private string postUrl = "http://localhost:51460/Api/MaintanceLog";
        [SerializeField] private TMP_InputField personnelNameField;
        [SerializeField] private TMP_InputField descriptionField;
        [SerializeField] private TMP_InputField detailsOfRepairField;
        [SerializeField] private TMP_InputField materialCostField;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button sendButton;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            var tab = new ContentTab_Model($"Form Test",
                () =>
                {
                    gameObject.SetActive(true);
                },
                null);

            ContentTabBar.Instance.AddTab(tab);

            sendButton.onClick.AddListener(SaveForm);
            closeButton.onClick.AddListener(() =>
            {
                ContentTabBar.Instance.ActiveTab = null;
            });
        }

        #endregion

        private void SaveForm()
        {
            float.TryParse(materialCostField.text, out var cost);

            var model = new MaintanceLog()
            {
                PersonnelName = personnelNameField.text,
                Description = descriptionField.text,
                Details = detailsOfRepairField.text,
                Cost = cost
            };

            sendButton.interactable = false;
            StartCoroutine(PostLog(model));
        }

        private IEnumerator PostLog(MaintanceLog log)
        {
            var apiRequest = UnityWebRequest.Post(postUrl, JsonConvert.SerializeObject(log));

            yield return apiRequest.SendWebRequest();

            materialCostField.text = string.Empty;
            personnelNameField.text = string.Empty;
            descriptionField.text = string.Empty;
            detailsOfRepairField.text = string.Empty;

            sendButton.interactable = true;
            ConsoleGUI.Instance.WriteLn("Saved successfully. You are the best. ;)");
        }
    }
}
