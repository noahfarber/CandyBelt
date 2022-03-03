using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debugger : MonoBehaviour
{
    public static Debugger Instance;

    public bool Logs = true;
    public bool Erorrs = true;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void Log(string message)
    {
        if (Logs)
        {
            Debug.Log(message);
        }
    }

    public void LogError(string message)
    {
        if (Erorrs)
        {
            Debug.LogError(message);
        }
    }
}
