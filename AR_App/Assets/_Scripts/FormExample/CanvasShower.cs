using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasShower : MonoBehaviour
{
    #region Show in editor

    [SerializeField] private GameObject canvas;

    #endregion

    void Start()
    {
        var contentHandler = transform.parent.GetComponent<ContentHandler>();
        var cavasObject = Instantiate(canvas);
        cavasObject.SetActive(false);

        contentHandler.OnTrackingStart += () => cavasObject.SetActive(true);
    }
}
