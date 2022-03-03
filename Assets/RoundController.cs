using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundController : MonoBehaviour
{
    public int ItemsToSpawn = 10;
    public int ItemsSpawned = 0;
    public TextMeshProUGUI ScoreText;
    private int ItemsCollected = 0;

    public void RestartRound()
    {
        ItemsSpawned = 0;
        ItemsCollected = 0;
        UpdateScore();
    }

    public bool CanSpawnItems()
    {
        return ItemsSpawned < ItemsToSpawn;
    }

    public void ItemSpawned()
    {
        ItemsSpawned++;
    }

    public void CollectItem()
    {
        ItemsCollected++;
        UpdateScore();
    }

    public void UpdateScore()
    {
        ScoreText.text = $"SCORE: {((float)ItemsCollected / (float)ItemsToSpawn):P0}";
    }
}
