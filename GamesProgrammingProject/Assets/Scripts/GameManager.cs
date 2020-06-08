using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using UnityEngine;

public sealed class GameManager
{
    private static GameManager instance = new GameManager();
    public UserSession currentUser;


    static GameManager()
    {
    }

    private GameManager()
    {
    }

    public static GameManager GmInstance
    {
        get { return instance; }
    }
    
    public void SetUser(UserSession authenticatedUser)
    {
        currentUser = authenticatedUser;
    }


}

