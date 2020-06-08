using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class UserSession
{
    readonly int UserID;
    readonly string Username;

    public UserSession(int ID, string name)
    {
        UserID = ID;
        Username = name;
    }

    public int GetID()
    {
        return UserID;
    }

    public string GetUsername()
    {
        return Username;
    }

}
