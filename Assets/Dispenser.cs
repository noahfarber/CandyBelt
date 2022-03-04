using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dispenser : MonoBehaviour
{
    [SerializeField] private ConveyorBelt AssociatedBelt;
    [SerializeField] private GameObject[] _ConveyorItemPrefabs;
    [SerializeField] private Transform _SpawnPosition;
    [SerializeField] private Transform _InactiveItems;
    [HideInInspector] public List<ConveyorItem> InactiveItemList = new List<ConveyorItem>();
    private int _ItemsToGenerateOnStart = 5;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
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

    private void CreateItem()
    {
        GameObject prefab = Instantiate(_ConveyorItemPrefabs[Random.Range(0, _ConveyorItemPrefabs.Length)], transform);
        InactiveItemList.Add(prefab.GetComponent<ConveyorItem>());
        prefab.SetActive(false);
    }

    public void SpawnItem()
    {
        if (InactiveItemList.Count <= 0)
        {
            CreateItem();
        }

        ConveyorItem itemToSpawn = InactiveItemList[Random.Range(0, InactiveItemList.Count)];
        itemToSpawn.transform.position = _SpawnPosition.position;
        itemToSpawn.gameObject.SetActive(true);
        AssociatedBelt.AddItem(itemToSpawn);
        if (InactiveItemList.Contains(itemToSpawn)) { InactiveItemList.Remove(itemToSpawn); }

        Debugger.Instance.Log($"Spawning a candy to conveyor belt: {AssociatedBelt.name}");
    }

    public void Recycle(ConveyorItem item)
    {
        InactiveItemList.Add(item);
        item.gameObject.SetActive(false);
        item.transform.parent = _InactiveItems;
    }
}
