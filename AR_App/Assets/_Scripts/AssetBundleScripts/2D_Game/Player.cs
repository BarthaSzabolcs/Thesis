using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Show in editor

    [Header("Jump:")]
    [SerializeField] private Vector2 jumpArea;
    [SerializeField] private Vector2 groundCheck;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private string groundName;

    #endregion
    #region Hide in editor

    private Rigidbody2D rbody;

    #endregion

    #region Unity Callbacks

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (/*Input.touchCount > 0*/Input.GetMouseButtonDown(0))
        {
            var colliders = Physics2D.OverlapBoxAll(transform.TransformPoint(groundCheck), jumpArea, 0);
            if (colliders.Any(c => c.name == groundName))
            {
                rbody.velocity = Vector2.up * jumpVelocity;
            }
        }        
    }

    #endregion

}
