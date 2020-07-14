using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

//Singleton Design, thread safe lock, non-lazy
public sealed class UserSessionManager
{

    private static UserSession _userSessionInstance;

    private static readonly object _lock = new object();



    private class UserSession
    {
        public readonly int userID;
        public readonly string username;

        public UserSession(int ID, string name)
        {
            userID = ID;
            username = name;
        }
    }

    private UserSessionManager() 
    { 
    
    }

    public static void CreateUserSessionInstance(int ID, string name)
    {
        if(_userSessionInstance == null)
        {
            lock (_lock)
            {
                if (_userSessionInstance == null)
                {
                    _userSessionInstance = new UserSession(ID, name);
                }
            }
        }
    }

    public static int GetID()
    {
        try
        {
            return _userSessionInstance.userID;
        }
        catch (NullReferenceException)
        {
            Debug.Log("Error: No valid user session.");
            //-1 as an error value
            //TODO: Add enum for errors/error checking/tests
            return -1;
        }
    }

    public static string GetUsername()
    {
        try
        {
            return _userSessionInstance.username;
        }
        catch (NullReferenceException)
        {
            Debug.Log("Error: No valid user session." );
            return null;
        }
    }

}
