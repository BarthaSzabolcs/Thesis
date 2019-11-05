using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private ConnectionConfig connectionConfig;

    public static ConnectionManager Instance { get; set; }

    public string Con { get; set; }

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;

            Con = connectionConfig.server;
            ipInputField.text = Con;
        }
        else
        {
            Destroy(this);
        }
    }

    public void RefreshConnection()
    {
        Con = ipInputField.text;
    }
}
