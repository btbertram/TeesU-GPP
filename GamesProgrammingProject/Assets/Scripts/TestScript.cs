using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.Security.Cryptography;

public class TestScript : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {

        //SQLite doesn't work directly with LINQ. As a result, we'll need to work with more generic implementaitons.

        string connectionString = "URI=file:" + Application.dataPath + "/prototypeDB.db";
        IDbConnection dbConnection;
        dbConnection = new SqliteConnection(connectionString);

        dbConnection.Open();
        IDbCommand dbCommand;
        dbCommand = dbConnection.CreateCommand();


        string selectQuery = "SELECT * FROM TestItem";

        dbCommand.CommandText = selectQuery;
        IDataReader dataReader = dbCommand.ExecuteReader();

        while (dataReader.Read())
        {
            string itemName = dataReader.GetString(1);
            int wealthValue = dataReader.GetInt32(2);

            int id = dataReader.GetInt32(0);


            Debug.Log("Name: " + itemName + ". Value: " + wealthValue + ". ID: " + id +".");
        }

        string testusername = "testUser";
        string testpasscode = "12345";

        CreateAccount(testusername, testpasscode, dbConnection);

        dbCommand.CommandText = "SELECT * FROM UserAccounts";

        dbCommand.ExecuteReader();

        while (dataReader.Read())
        {
            string userName = dataReader.GetString(1);
            var salt = dataReader.GetValue(2);
            var hash = dataReader.GetValue(3);

            int id = dataReader.GetInt32(0);


            Debug.Log("User: " + userName + ". Salt: " + salt + ". ID: " + id + ". Hash: " + hash + ".");
        }


        dataReader.Close();
        dataReader = null;
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateAccount(string newUsername, string newPasscode, IDbConnection dbConnection)
    {
        //Generate Salt
        System.Guid guid = System.Guid.NewGuid();

        //Combine salt with newPasscode

        byte[] encodedPasscode = System.Text.ASCIIEncoding.ASCII.GetBytes(newPasscode + guid.ToString() + newUsername);

        //Hash Salted Passcode
        ///Create Hashgen

        SHA256 sHA256 = SHA256.Create();

        string finalHash = sHA256.ComputeHash(encodedPasscode).ToString();

      
        //Insert query to database - new entry in user account table
        ///Sends User and Hash
        ///Need Database connection to do this - Should have a class that saves connection. Scriptable object?

        string insertQuery = "INSERT into UserAccounts(username, hash, salt) VALUES( '" + newUsername + "', '" + finalHash + "', '" + guid + "');";
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = insertQuery;

        dbCommand.ExecuteNonQuery();
        Debug.Log("Attempted Account created");

        //DB side - Trigger after insert: Create new User data table
    }

    private void AuthAccount(string username, string passcode)
    {
        //Select Query to database - retrive salt + hash based on Username

        //Combine salt + passcode

        //Hash salted passcode

        //compare generated hash vs DB stored hash

        //Match: Success, login
        //Fail: Display error message
    }


}
