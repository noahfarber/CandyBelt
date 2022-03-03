using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public RoundController RoundManager;
    public ConveyorBelt MainBelt;
    public ConveyorBelt[] ForwardBelts;

    [HideInInspector] public GameStates State = GameStates.Playing;

    private float _MinSpawnTime = 1f;
    private float _MaxSpawnTime = 3f;

    private float _SpawnTimer = 0f;
    private float _TimeToSpawn = 0f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        UpdateSpawnTime();
        RoundManager.RestartRound();
    }

    void Update()
    {
        if (State == GameStates.Playing)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnItem();
            }

            if (RoundManager.CanSpawnItems())
            {
                CheckForSpawn();
            }

            ProcessBelts();
        }
    }

    public void CollectItem(ConveyorItem item)
    {
        RoundManager.CollectItem();
    }

    public void TryChangeBelt(ConveyorItem item)
    {
        if (item.TouchingBelt != null && item.TouchingBelt != item.CurrentBelt)
        {
            item.CurrentBelt.RemoveItem(item);
            item.TouchingBelt.AddItem(item);
        }
    }

    public void UpdatePauseState()
    {
        if(State == GameStates.Playing)
        {
            State = GameStates.Paused;
        }
        else if(State == GameStates.Paused)
        {
            State = GameStates.Playing;
        }

        Debugger.Instance.Log($"State Changed: {State}");
    }

    private void ProcessBelts()
    {
        MainBelt.Process();

        for (int i = 0; i < ForwardBelts.Length; i++)
        {
            ForwardBelts[i].Process();
        }
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

    private void UpdateSpawnTime()
    {
        _TimeToSpawn = Random.Range(_MinSpawnTime, _MaxSpawnTime);
    }

    private void SpawnItem()
    {
        MainBelt.SpawnItem();
        UpdateSpawnTime();
        RoundManager.ItemSpawned();
        _SpawnTimer = 0f;
    }
}

public enum GameStates
{
    MainMenu = 0,
    Playing = 1,
    Paused = 2
}

