using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public CandyType AcceptableTypes = CandyType.None;
    public Vector3 Direction = Vector3.right;

    public Transform StartPosition;
    public Transform EndPosition;
    public float Speed = 5f;
    public float CorrectiveSpeed = 2f;

    public void ReceiveItem(ConveyorItem item)
    {
        item.CurrentBelt = this;
        item.transform.parent = transform;
        item.transform.localScale = Vector3.one;
        item.gameObject.SetActive(true);
    }

    public bool IsMainBelt()
    {
        return Direction == Vector3.right;
    }

    public bool IsForwardBelt()
    {
        return Direction == Vector3.up;
    }
}
