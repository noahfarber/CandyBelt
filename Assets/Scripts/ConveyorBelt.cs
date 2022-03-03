using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public CandyType AcceptableTypes = CandyType.None;
    public Vector3 Direction = Vector3.right;
    [HideInInspector] public List<ConveyorItem> Items = new List<ConveyorItem>();
    [HideInInspector] public List<ConveyorItem> InactiveItemPool = new List<ConveyorItem>();
    [SerializeField] private bool _CanSpawnItems = false;

    [SerializeField] private GameObject _ConveyorItemPrefab;
    [SerializeField] private Transform _StartPosition;
    [SerializeField] private Transform _EndPosition;
    [SerializeField] private float _Speed = 5f;

    private int _ItemsToGenerateOnStart = 5;
    private Vector3 positionHolder = Vector3.zero; // Used for item position calculation
    private Vector3 scaleHolder = Vector3.zero; // Used for item position calculation

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    public void Process()
    {
        MoveBeltItems();
    }

    private void Init()
    {
        if(InactiveItemPool.Count == 0 && _CanSpawnItems)
        {
            for (int i = 0; i < _ItemsToGenerateOnStart; i++)
            {
                CreateItem();
            }
        }
    }

    private void CreateItem()
    {
        GameObject prefab = Instantiate(_ConveyorItemPrefab, transform);
        InactiveItemPool.Add(prefab.GetComponent<ConveyorItem>());
        prefab.SetActive(false);
    }

    public void SpawnItem()
    {
        if(!_CanSpawnItems)
        {
            return;
        }

        if(InactiveItemPool.Count <= 0)
        {
            CreateItem();
        }

        ConveyorItem newItem = InactiveItemPool[Random.Range(0, InactiveItemPool.Count)];
        newItem.transform.position = _StartPosition.position;
        newItem.gameObject.SetActive(true);
        AddItem(newItem);
        Debugger.Instance.Log("Spawning a candy");
    }

    public void AddItem(ConveyorItem item)
    {
        if (InactiveItemPool.Contains(item)) { InactiveItemPool.Remove(item); }

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

                if(Direction == Vector3.up)
                {
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

        if (Direction == Vector3.right)
        {
            rtn = positionHolder.x >= _EndPosition.position.x;
        }
        else if (Direction == Vector3.up)
        {
            rtn = positionHolder.y >= _EndPosition.position.y;
        }

        return rtn;
    }

    private float DistanceTraveled()
    {
        float rtn = 0f;

        if (Direction == Vector3.right)
        {
            rtn = positionHolder.x / _EndPosition.position.x;
        }
        else if (Direction == Vector3.up)
        {
            rtn = positionHolder.y / _EndPosition.position.y;
        }

        return rtn;
    }

    private void ItemCollected(ConveyorItem item)
    {
        if(AcceptableTypes.HasFlag(item.Type))
        {
            GameController.Instance.CollectItem(item);
        }

        RecycleItem(item);
    }

    private void RecycleItem(ConveyorItem item)
    {
        Debugger.Instance.Log($"Belt {gameObject.name} is recycling item: {item.transform.name}");
        Items.Remove(item);
        InactiveItemPool.Add(item);
        item.gameObject.SetActive(false);
    }
}
