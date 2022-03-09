using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public interface IInputManager
    {
        public void Init(GameController controller);
        public void ProcessInput();
    }

    public class InputManager : MonoBehaviour, IInputManager
    {
        private GameController _Controller;

        public void Init(GameController controller)
        {
            _Controller = controller;
        }

        public void ProcessInput()
        {
            // Add custom input handling here...
        }
    }

}