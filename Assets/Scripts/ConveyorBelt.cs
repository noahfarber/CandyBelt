using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [HideInInspector] public List<ConveyorItem> Items = new List<ConveyorItem>();
    [HideInInspector] public List<ConveyorItem> InactiveItemPool = new List<ConveyorItem>();
    [SerializeField] private GameObject _ConveyorItemPrefab;
    [SerializeField] private Transform _StartPosition;
    [SerializeField] private Transform _EndPosition;
    [SerializeField] private float _Speed = 5f;

    private int _ItemsToGenerateOnStart = 5;
    private bool debugLoop = true;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnItem();
        }

        MoveBeltItems();
    }

    private void Init()
    {
        if(InactiveItemPool.Count == 0)
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
        if(InactiveItemPool.Count <= 0)
        {
            CreateItem();
        }

        ConveyorItem newItem = InactiveItemPool[Random.Range(0, InactiveItemPool.Count)];
        newItem.transform.position = _StartPosition.position;
        newItem.gameObject.SetActive(true);
        AddItem(newItem);
    }

    public void AddItem(ConveyorItem item)
    {
        if(Items.Contains(item))
        {
            Items.Remove(item);
            Debug.LogError($"Added {item.name} to conveyor belt but belt already contains {item.name}");
        }

        Items.Add(item);
    }


    private Vector3 positionHolder = Vector3.zero;

    private void MoveBeltItems()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            ConveyorItem item = Items[i];
            positionHolder = item.transform.position;
            positionHolder.x += _Speed * Time.deltaTime;
            if (positionHolder.x < _EndPosition.position.x)
            {
                positionHolder.x += _Speed * Time.deltaTime;
            }
            else
            {
                if (debugLoop)
                {
                    positionHolder.x = _StartPosition.position.x;
                }
                else
                {
                    RecycleItem(item);
                }
            }
            item.transform.position = positionHolder;
        }
    }

    private void RecycleItem(ConveyorItem item)
    {
        Items.Remove(item);
        InactiveItemPool.Add(item);
    }
}
