using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public CandyType AcceptableTypes = CandyType.None;
    public Vector3 Direction = Vector3.right;
    [HideInInspector] public List<ConveyorItem> Items = new List<ConveyorItem>();

    [SerializeField] private Dispenser RecycleDispenser;
    [SerializeField] private Transform _StartPosition;
    [SerializeField] private Transform _EndPosition;
    [SerializeField] private float _Speed = 5f;
    private float _CorrectiveSpeed = 2f;

    private Vector3 positionHolder = Vector3.zero; // Used for item position calculation
    private Vector3 scaleHolder = Vector3.zero; // Used for item position calculation

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void Process()
    {
        MoveBeltItems();
    }


    public void AddItem(ConveyorItem item)
    {
        if (Items.Contains(item))
        {
            Items.Remove(item);
            Debugger.Instance.LogError($"Added {item.name} to conveyor belt but belt already contains {item.name}");
        }

        item.CurrentBelt = this;
        item.transform.parent = transform;
        Items.Add(item);
    }

    public void RemoveItem(ConveyorItem item)
    {
        if(Items.Contains(item))
        {
            Items.Remove(item);
        }
    }

    private void MoveBeltItems()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            ConveyorItem item = Items[i];
            positionHolder = item.transform.position;
            scaleHolder = item.transform.localScale;

            if (HasReachedEnd())
            {
                ItemCollected(item);
            }
            else
            {
                positionHolder += _Speed * Time.deltaTime * Direction;

                if(ForwardBelt())
                {
                    // Move item closer to center of exit
                    if(positionHolder.x < _EndPosition.position.x - .1f)
                    {
                        positionHolder.x += Time.deltaTime * _CorrectiveSpeed;
                    }
                    else if(positionHolder.x > _EndPosition.position.x + .1f)
                    {
                        positionHolder.x -= Time.deltaTime * _CorrectiveSpeed;
                    }

                    scaleHolder.x = 1f - (.5f * DistanceTraveled());
                    scaleHolder.y = 1f - (.5f * DistanceTraveled());
                }
            }

            item.transform.position = positionHolder;
            item.transform.localScale = scaleHolder;
        }
    }

    private bool HasReachedEnd()
    {
        bool rtn = false;

        if (MainBelt())
        {
            rtn = positionHolder.x >= _EndPosition.position.x;
        }
        else if (ForwardBelt())
        {
            rtn = positionHolder.y >= _EndPosition.position.y;
        }

        return rtn;
    }

    private float DistanceTraveled()
    {
        float rtn = 0f;

        if (MainBelt())
        {
            rtn = positionHolder.x / _EndPosition.position.x;
        }
        else if (ForwardBelt())
        {
            rtn = positionHolder.y / _EndPosition.position.y;
        }

        return rtn;
    }

    private bool MainBelt()
    {
        return Direction == Vector3.right;
    }

    private bool ForwardBelt()
    {
        return Direction == Vector3.up;
    }

    private void ItemCollected(ConveyorItem item)
    {
        if(AcceptableTypes.HasFlag(item.Type))
        {
            GameController.Instance.CollectItem(item);
        }

        RecycleItem(item);
    }

    // Recycle item back to dispenser if one exists, otherwise do a rudimentary destroy
    private void RecycleItem(ConveyorItem item)
    {
        if(RecycleDispenser != null)
        {
            Items.Remove(item);
            RecycleDispenser.Recycle(item);
            Debugger.Instance.Log($"Belt {gameObject.name} is RECYCLING item: {item.transform.name}");
        }
        else
        {
            Destroy(item.gameObject);
            Debugger.Instance.Log($"Belt {gameObject.name} is DESTROYING item: {item.transform.name}");
        }
    }
}
