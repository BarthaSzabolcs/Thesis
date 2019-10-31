using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URLCollection : ScriptableObject
{
    #region Show in editor

    [SerializeField] private ConnectionConfig connection;

    [Header("Api endpoints:")]
    [SerializeField] private string recognizedObject;
    
    #endregion    
}
