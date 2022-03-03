using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundController : MonoBehaviour
{
    public int ItemsToSpawn = 10;
    public int ItemsSpawned = 0;
    private int ItemsCollected = 0;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI ItemsLeftText;

    public void RestartRound()
    {
        ItemsSpawned = 0;
        ItemsCollected = 0;
        UpdateUI();
    }

    public bool CanSpawnItems()
    {
        return ItemsSpawned < ItemsToSpawn;
    }

    public void ItemSpawned()
    {
        ItemsSpawned++;
        UpdateUI();
    }

    public void CollectItem()
    {
        ItemsCollected++;
        UpdateUI();
    }

    public void UpdateUI()
    {
        ScoreText.text = $"SCORE: {((float)ItemsCollected / (float)ItemsToSpawn):P0}";
        ItemsLeftText.text = "CANDIES LEFT: " + (ItemsToSpawn - ItemsSpawned).ToString();
    }
}
