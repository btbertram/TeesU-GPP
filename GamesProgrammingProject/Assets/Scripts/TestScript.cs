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

        string testusername = "testUser1";
        string testpasscode = "12345";

        Debug.Log(VerifyAccount(testusername, testpasscode, dbConnection));

        //dataReader.Close();
        //dataReader = null;
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DebugCode(IDbConnection dbConnection)
    {
        string selectQuery = "SELECT * FROM TestItem";
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = selectQuery;
        IDataReader dataReader = dbCommand.ExecuteReader();

        while (dataReader.Read())
        {
            string itemName = dataReader.GetString(1);
            int wealthValue = dataReader.GetInt32(2);

            int id = dataReader.GetInt32(0);


            Debug.Log("Name: " + itemName + ". Value: " + wealthValue + ". ID: " + id + ".");
        }
        dataReader.Close();

        dbCommand.CommandText = "SELECT * FROM UserAccounts";

        //IDataReader dataReader = dbCommand.ExecuteReader();

        //Note that this line will currently fail without an active data reader
        while (dataReader.Read())
        {
            string userName = dataReader.GetString(1);
            var salt = ByteArrayToString(dataReader.GetValue(2) as byte[]);
            var hash = ByteArrayToString(dataReader.GetValue(3) as byte[]);

            int id = dataReader.GetInt32(0);


            Debug.Log("User: " + userName + ". Salt: " + salt + ". ID: " + id + ". Hash: " + hash + ".");
        }
    }

    private void CreateAccount(string newUsername, string newPasscode, IDbConnection dbConnection)
    {
        //Generate Salt
        System.Guid guid = System.Guid.NewGuid();

        //Combine salt with newPasscode

        byte[] encodedPasscode = System.Text.Encoding.ASCII.GetBytes(newPasscode + guid.ToString() + newUsername);

        //Hash Salted Passcode
        ///Create Hashgen

        SHA256 sHA256 = SHA256.Create();

        byte[] computedHash = sHA256.ComputeHash(encodedPasscode);
        string finalHash = ByteArrayToString(computedHash);
      
        //Insert query to database - new entry in user account table
        ///Sends User and Hash
        ///Need Database connection to do this - Should have a class that saves connection. Scriptable object?

        string insertQuery = "INSERT into UserAccounts(username, hash, salt) VALUES( '" + newUsername + "', '" + finalHash + "', '" + guid + "');";
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = insertQuery;

        dbCommand.ExecuteNonQuery();
        Debug.Log("Attempted Account creation");

        //DB side - Trigger after insert: Create new User data table
    }

    private bool VerifyAccount(string username, string passcode, IDbConnection dbConnection)
    {
        SHA256 sHA256 = SHA256.Create();
        string selectQuery = "SELECT salt, hash FROM UserAccounts;";
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = selectQuery;
        IDataReader dataReader = dbCommand.ExecuteReader();
        string salt = "";
        string hash = "";

        while (dataReader.Read())
        {
            salt = ByteArrayToString(dataReader.GetValue(0) as byte[]);
            hash = ByteArrayToString(dataReader.GetValue(1) as byte[]);
        }

        byte[] encodedPasscode = System.Text.Encoding.ASCII.GetBytes(passcode + salt + username);
        byte[] computedHash = sHA256.ComputeHash(encodedPasscode);

        if(hash == ByteArrayToString(computedHash))
        {
            Debug.Log("Hash From db is: " + hash);
            Debug.Log("Generated Hash is: " + ByteArrayToString(computedHash));
            return true;
        }
        else
        {
            Debug.Log("Salt from db is: " + salt);
            Debug.Log("Hash From db is: " + hash);
            Debug.Log("Generated Hash is: " + ByteArrayToString(computedHash));
            return false;
        }

    }

    string ByteArrayToString(byte[] hash)
    {
        string hashString = "";

        foreach (byte x in hash)
        {
            hashString += x.ToString();
        }

        return hashString;
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
