using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [SerializeField] private GameObject[] _ConveyorItemPrefabs;
    [SerializeField] private Transform _InactiveItems;
    [HideInInspector] public List<ConveyorItem> InactiveItemList = new List<ConveyorItem>();
    [HideInInspector] public List<ConveyorItem> ActiveItemList = new List<ConveyorItem>();
    [SerializeField] private int _ItemsToGenerateOnStart = 50;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }    
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < ActiveItemList.Count; i++)
        {
            ActiveItemList[i].ProcessUpdate();
        }
    }

    public bool AllItemsInactive()
    {
        return ActiveItemList.Count == 0;
    }

    private void Init()
    {
        if (InactiveItemList.Count == 0)
        {
            for (int i = 0; i < _ItemsToGenerateOnStart; i++)
            {
                CreateItem();
            }
        }
    }

    public ConveyorItem GetRandomItem()
    {
        if (InactiveItemList.Count <= 0)
        {
            CreateItem();
        }

        ConveyorItem item = InactiveItemList[Random.Range(0, InactiveItemList.Count)];
        InactiveItemList.Remove(item);

        return item;
    }

    public void Recycle(ConveyorItem item)
    {
        item.gameObject.SetActive(false);
        item.transform.parent = _InactiveItems;
        item.transform.localScale = Vector3.one;
        item.transform.position = _InactiveItems.position;

        if (InactiveItemList.Contains(item)) { InactiveItemList.Remove(item); Debugger.Instance.LogError("Inactive item already in pool..."); }
        if (ActiveItemList.Contains(item)) { ActiveItemList.Remove(item); }

        InactiveItemList.Add(item);
    }

    private void CreateItem()
    {
        GameObject prefab = Instantiate(_ConveyorItemPrefabs[Random.Range(0, _ConveyorItemPrefabs.Length)], _InactiveItems);
        InactiveItemList.Add(prefab.GetComponent<ConveyorItem>());
        prefab.transform.position = _InactiveItems.position;
        prefab.SetActive(false);
    }
}
