using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class CandyGameController : GameController
{
    public static CandyGameController Instance;

    public RoundController RoundManager;
    public Dispenser ItemDispenser;
    public ConveyorBelt[] ConveyorBelts;

    private float _MinSpawnTime = 1f;
    private float _MaxSpawnTime = 3f;

    private float _SpawnTimer = 0f;
    private float _TimeToSpawn = 0f;

    private float _RoundResetTimer = 0f;
    private float _TimeBetweenRounds = 5f;

    private GameStates CandyState;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public override void Init()
    {
        UpdateNextSpawnTime();
        RoundManager.StartNextRound(0);
        base.Init();
    }

    public override void Update()
    {
        if (CandyState == GameStates.Playing)
        {
            if (RoundManager.CanSpawnItems())
            {
                CheckForSpawn();
            }
            else if(ItemManager.Instance.AllItemsInactive())
            {
                WaitForNextRound();
            }
        }
    }

    public void CollectItem(ConveyorItem item)
    {
        if (item.CurrentBelt.AcceptableTypes.HasFlag(item.Type))
        {
            RoundManager.CollectItem();
        }

        // Last item spawned
        if (!RoundManager.CanSpawnItems() && ItemManager.Instance.AllItemsInactive())
        {
            StartNextRoundDelay();
        }
    }

    public void TryChangeBelt(ConveyorItem item)
    {
        if (item.TouchingBelt != null && item.TouchingBelt != item.CurrentBelt)
        {
            item.TouchingBelt.ReceiveItem(item);
        }
    }

    public void UpdatePauseState()
    {
        if(CandyState == GameStates.Playing)
        {
            CandyState = GameStates.Paused;
        }
        else if(CandyState == GameStates.Paused)
        {
            CandyState = GameStates.Playing;
        }

        Debugger.Instance.Log($"State Changed: {CandyState}");
    }

    private void CheckForSpawn()
    {
        if(_SpawnTimer >= _TimeToSpawn)
        {
            SpawnItem();
        }
        else
        {
            _SpawnTimer += Time.deltaTime;
        }
    }

    private void StartNextRoundDelay()
    {
        _RoundResetTimer = 0f;
        RoundManager.DisplayMessage($"ROUND {RoundManager.RoundNumber + 1} COMPLETE!", 1f);
    }

    private void WaitForNextRound()
    {
        if(_RoundResetTimer >= _TimeBetweenRounds)
        {
            RoundManager.StartNextRound();
        }
        else
        {
            _RoundResetTimer += Time.deltaTime;
            if(_RoundResetTimer > 1.5f)
            {
                RoundManager.DisplayMessage($"ROUND {RoundManager.RoundNumber + 2} STARTING IN {(int)(_TimeBetweenRounds - _RoundResetTimer)}s", 999f);
            }
        }
    }

    private void UpdateNextSpawnTime()
    {
        _SpawnTimer = 0f;
        _TimeToSpawn = Random.Range(_MinSpawnTime, _MaxSpawnTime);
    }

    private void SpawnItem()
    {
        ItemDispenser.DropItem();
        RoundManager.ItemSpawned();
        UpdateNextSpawnTime();
    }
}

public enum GameStates
{
    MainMenu = 0,
    Playing = 1,
    Paused = 2
}

