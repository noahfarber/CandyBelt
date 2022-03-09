using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

// Sample implementation of the input controller system
public class TicTacToeInputManager : IInputManager
{
    private GameController _Controller;

    public void Init(GameController controller)
    {
        _Controller = controller;
    }

    public void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_Controller.IsPlayingGame())
            {
                _Controller.PauseGame();
            }
            else if (_Controller.IsPaused())
            {
                _Controller.UnpauseGame();
            }
        }
    }
}