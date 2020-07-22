using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;

//Singleton, Thread-Safe, Lazy
public sealed class GameManager
{

    private static GameManager _gameManagerInstance;

    private static readonly object _lock = new object();

    private GameManager()
    {
        
    }

    public static GameManager GetGMInstance()
    {

        if (_gameManagerInstance == null)
        {
            lock (_lock)
            {
                if (_gameManagerInstance == null)
                {
                    _gameManagerInstance = new GameManager();
                }
            }
        }
        return _gameManagerInstance;
    }

    public void UnityApplicationQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    
}

