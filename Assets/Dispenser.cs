using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Dispenser : MonoBehaviour
{
    public ItemManager ItemManager;

    [SerializeField] private ConveyorBelt AssociatedBelt;
    [SerializeField] private Animator DispenserAnimation;
    [SerializeField] private Transform _SpawnPosition;
    [SerializeField] private Transform _BeltStartPosition;
    private ConveyorItem _NextItemToSpawn;
    private ConveyorItem _ItemSpawning;

    // Start is called before the first frame update
    void Start()
    {
        UpdateNextItem();
    }

    private void UpdateNextItem()
    {
        _NextItemToSpawn = ItemManager.GetRandomItem();
        _NextItemToSpawn.State = ItemState.Spawning;
        _NextItemToSpawn.gameObject.SetActive(true);
        _NextItemToSpawn.Sprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        _NextItemToSpawn.transform.position = _SpawnPosition.position;
        _NextItemToSpawn.transform.localScale = Vector3.zero;
        _NextItemToSpawn.transform.DOScale(1f, .25f);
    }

    public void DropItem()
    {
        DispenserAnimation.Play("Dispense");
        _ItemSpawning = _NextItemToSpawn;
        _ItemSpawning.gameObject.SetActive(true);
        _ItemSpawning.transform.DOMove(_BeltStartPosition.position, .5f).OnComplete(AttachItemToBelt);
        UpdateNextItem();

        Debugger.Instance.Log($"Spawning a candy to conveyor belt: {AssociatedBelt.name}");
    }

    private void AttachItemToBelt()
    {
        ItemManager.ActiveItemList.Add(_ItemSpawning);
        _ItemSpawning.Sprite.maskInteraction = SpriteMaskInteraction.None;
        _ItemSpawning.State = ItemState.Moving;
        AssociatedBelt.ReceiveItem(_ItemSpawning);
        _ItemSpawning = null;
    }

}
