using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundController : MonoBehaviour
{
    public int RoundNumber = 0;
    public int[] ItemsToSpawnPerRound;
    private int[] ItemsCollected;
    private int[] ItemsSpawnedThisRound;

    public int Score;
    public int ScorePerItem = 10;

    public TextMeshProUGUI GameMessageText;
    public TextMeshProUGUI RoundText;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI HighScoreText;
    public TextMeshProUGUI ItemsLeftText;

    private Color HighscoreDefaultColor;
    private float _GameMessageTimer = 0f;
    private float _GameMessageDuration;
    private string _GameMessage;

    private void Awake()
    {
        ItemsCollected = new int[ItemsToSpawnPerRound.Length];
        ItemsSpawnedThisRound = new int[ItemsToSpawnPerRound.Length];
        HighscoreDefaultColor = HighScoreText.color;
    }

    private void Update()
    {
        UpdateGameMessage();
    }

    public void DisplayMessage(string message, float timeSec)
    {
        _GameMessageTimer = 0f;
        _GameMessageDuration = timeSec;
        _GameMessage = message;
    }

    private void UpdateGameMessage()
    {
        if (_GameMessageTimer >= _GameMessageDuration)
        {
            GameMessageText.text = "";
        }
        else
        {
            GameMessageText.text = _GameMessage;
            _GameMessageTimer += Time.deltaTime;
        }
    }

    public void StartNextRound(int specificRound = -1)
    {
        if (specificRound != -1)
        {
            RoundNumber = specificRound;
        }
        else
        {
            RoundNumber++;
        }

        if(specificRound == 0)
        {
            ResetHighscore();
        }

        RestartCurrentRound();

        DisplayMessage($"ROUND {RoundNumber + 1} STARTING!", 1f);
    }

    public void RestartCurrentRound()
    {
        HighScoreText.color = HighscoreDefaultColor;
        ItemsSpawnedThisRound[RoundNumber] = 0;
        ItemsCollected[RoundNumber] = 0;
        UpdateUI();
    }

    public bool CanSpawnItems()
    {
        return ItemsSpawnedThisRound[RoundNumber] < ItemsToSpawnPerRound[RoundNumber];
    }

    public void ItemSpawned()
    {
        ItemsSpawnedThisRound[RoundNumber]++;
        UpdateUI();
    }

    public void CollectItem()
    {
        ItemsCollected[RoundNumber]++;
        Score += ScorePerItem * (RoundNumber + 1);
        UpdateUI();
    }

    private void UpdateUI()
    {
        RoundText.text = $"ROUND: {RoundNumber + 1}";
        ScoreText.text = $"SCORE: {Score}";
        ItemsLeftText.text = "CANDIES LEFT: " + (ItemsToSpawnPerRound[RoundNumber] - ItemsSpawnedThisRound[RoundNumber]).ToString();

        if (Score > PlayerPrefs.GetInt("Highscore"))
        {
            HighScoreText.color = ScoreText.color;
            HighScoreText.text = $"HIGH SCORE: {Score}";
            PlayerPrefs.SetInt("Highscore", Score);
        }
    }

    private void ResetHighscore()
    {
        HighScoreText.text = $"HIGH SCORE: {PlayerPrefs.GetInt("Highscore")}";
    }
}

