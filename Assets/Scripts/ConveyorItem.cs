using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConveyorItem : MonoBehaviour
{
    public CandyType Type = CandyType.None;

    [HideInInspector] public ConveyorBelt CurrentBelt;
    [HideInInspector] public ConveyorBelt TouchingBelt;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<SwipeZone>() != null)
        {
            SwipeZone zone = collision.transform.GetComponent<SwipeZone>();
            if (zone.AssociatedBelt.AcceptableTypes.HasFlag(Type))
            {
                TouchingBelt = zone.AssociatedBelt;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TouchingBelt = null;
    }
}
