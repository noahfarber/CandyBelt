using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Framework
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject MainMenu;
        [SerializeField] private GameObject PauseMenu;
        [SerializeField] private Button PlayButtom;

        [HideInInspector] public GameState State = GameState.MainMenu;

        public List<IInputManager> InputStack = new List<IInputManager>();

        private void Start()
        {
            Init();
        }

        public virtual void Init()
        {

        }

        public virtual void Update()
        {
            ProcessInputs();
        }

        public virtual void StartGame()
        {
            MainMenu.SetActive(false);
            PauseMenu.SetActive(false);
            State = GameState.PlayingGame;
        }

        public virtual void ReturnToMenu()
        {
            MainMenu.SetActive(true);
            PauseMenu.SetActive(false);
            State = GameState.MainMenu;
        }

        public virtual void PauseGame()
        {
            PauseMenu.SetActive(true);
            State = GameState.Paused;
        }

        public virtual void UnpauseGame()
        {
            PauseMenu.SetActive(false);
            State = GameState.PlayingGame;
        }

        public virtual void ExitApplication()
        {
            Application.Quit();
        }

        private void ProcessInputs()
        {
            for (int i = 0; i < InputStack.Count; i++)
            {
                InputStack[i].ProcessInput();
            }
        }

        #region Current State Check Calls
        public bool IsMainMenu()
        {
            return State == GameState.MainMenu;
        }

        public bool IsPlayingGame()
        {
            return State == GameState.PlayingGame;
        }

        public bool IsPaused()
        {
            return State == GameState.Paused;
        }

        public bool IsGameOver()
        {
            return State == GameState.GameOver;
        }
        #endregion

    }

    public enum GameState
    {
        MainMenu = 0,
        PlayingGame = 1,
        Paused = 2,
        GameOver = 4
    }
}