using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ConveyorItem : MonoBehaviour
{
    public CandyType Type = CandyType.None;
    public SpriteRenderer Sprite;

    [HideInInspector] public ItemState State = ItemState.Disabled;
    [HideInInspector] public ConveyorBelt CurrentBelt;
    [HideInInspector] public ConveyorBelt TouchingBelt;


    private Vector3 positionHolder = Vector3.zero; // Used for item position calculation
    private Vector3 scaleHolder = Vector3.one; // Used for item position calculation

    public void ProcessUpdate()
    {
        if (State == ItemState.Moving)
        {
            ProcessMove();
        }
    }

    private void ProcessMove()
    {
        positionHolder = transform.position;
        scaleHolder = transform.localScale;

        if (HasReachedEnd())
        {
            CollectItem();
        }
        else
        {
            positionHolder += CurrentBelt.Speed * Time.deltaTime * CurrentBelt.Direction;

            if (CurrentBelt.IsForwardBelt())
            {
                ApplyPositionCorrection();
                ApplyScaleByDistance();
            }
        }

        // Update item properties
        transform.position = positionHolder;
        transform.localScale = scaleHolder;
    }

    private void CollectItem()
    {
        State = ItemState.Disabled;
        ItemManager.Instance.Recycle(this);
        CandyGameController.Instance.CollectItem(this);
    }

    private void ApplyPositionCorrection()
    {
        // Move item closer to center of exit if misaligned
        if (positionHolder.x < CurrentBelt.EndPosition.position.x - .1f)
        {
            positionHolder.x += Time.deltaTime * CurrentBelt.CorrectiveSpeed;
        }
        else if (positionHolder.x > CurrentBelt.EndPosition.position.x + .1f)
        {
            positionHolder.x -= Time.deltaTime * CurrentBelt.CorrectiveSpeed;
        }
    }

    private void ApplyScaleByDistance()
    {
        scaleHolder.x = 1f - (.5f * DistanceTraveled());
        scaleHolder.y = 1f - (.5f * DistanceTraveled());
    }

    #region Position Checking

    private bool HasReachedEnd()
    {
        bool rtn = false;

        if (CurrentBelt.IsMainBelt())
        {
            rtn = positionHolder.x >= CurrentBelt.EndPosition.position.x;
        }
        else if (CurrentBelt.IsForwardBelt())
        {
            rtn = positionHolder.y >= CurrentBelt.EndPosition.position.y;
        }

        return rtn;
    }

    private float DistanceTraveled()
    {
        float rtn = 0f;

        if (CurrentBelt.IsMainBelt())
        {
            rtn = positionHolder.x / CurrentBelt.EndPosition.position.x;
        }
        else if (CurrentBelt.IsForwardBelt())
        {
            rtn = positionHolder.y / CurrentBelt.EndPosition.position.y;
        }

        return rtn;
    }
    #endregion

    #region Collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<SwipeZone>() != null)
        {
            SwipeZone zone = collision.transform.GetComponent<SwipeZone>();
            TouchingBelt = zone.AssociatedBelt;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        TouchingBelt = null;
    }
    #endregion
}

public enum ItemState
{
    Disabled = 0,
    Spawning = 1,
    Moving = 2
}
