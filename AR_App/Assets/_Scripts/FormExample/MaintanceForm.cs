using ContentBar;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MaintanceForm : MonoBehaviour
{
    #region Show in editor

    [SerializeField] private TMP_InputField personnelNameField;
    [SerializeField] private TMP_InputField descriptionField;
    [SerializeField] private TMP_InputField detailsOfRepairField;
    [SerializeField] private TMP_InputField materialCostField;

    #endregion

    #region Unity Callbacks

    private void Start()
    {
        var tab = new ContentTab_Model($"Form Test", null, null);

        ContentTabBar.Instance.AddTab(tab);
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

        // API CALL
        Debug.Log("Saved successfully.");
    }


}

public class MaintanceLog
{
    public string PersonnelName { get; set; }
    public string Description { get; set; }
    public string Details { get; set; }
    public float Cost { get; set; }
}
