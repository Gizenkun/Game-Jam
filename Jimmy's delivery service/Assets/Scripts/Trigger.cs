using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Trigger : MonoBehaviour
{
    public Action<Collider2D> TriggerEnterAction;
    public Action<Collider2D> TriggerStayAction;
    public Action<Collider2D> TriggerExitAction;

    private Collider2D collider = null;

    public bool Active
    {
        set
        {
            if(collider == null)
            {
                collider = GetComponent<Collider2D>();
            }
            collider.enabled = value;
        }
    }

    private void Awake()
    {
        Active = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TriggerEnterAction?.Invoke(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TriggerStayAction?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TriggerExitAction?.Invoke(collision);
    }
}
