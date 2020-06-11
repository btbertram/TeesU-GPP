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

        /*
         * This code is used as a testing mechanism for now. Attached to an object in unity,
         * It automatically creates a new account, putting it in the database, then reading it back out
         * by trying to log into it.
        */


        //SQLite doesn't work directly with LINQ. As a result, we'll need to work with more generic implementaitons.

        string connectionString = "URI=file:" + Application.dataPath + "/GameDB.db";
        IDbConnection dbConnection;
        dbConnection = new SqliteConnection(connectionString);

        dbConnection.Open();
        IDbCommand dbCommand;
        dbCommand = dbConnection.CreateCommand();

        string testusername = "testUser125";
        string testpasscode = "1273457";

        //CreateAccount(testusername, testpasscode, dbConnection);

        GrantAuth(VerifyAccount(testusername, testpasscode, dbConnection), testusername, dbConnection);

        Debug.Log(UserSessionSingleton.GetUsername());
        Debug.Log(UserSessionSingleton.GetID());

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

    /// <summary>
    /// Prints text debug logs in Unity to test fetching and displaying information from the SQLite database.
    /// </summary>
    /// <param name="dbConnection">The Database connection object for the database in use.</param> 
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
            var salt = ByteArrayContentsToString(dataReader.GetValue(2) as byte[]);
            var hash = ByteArrayContentsToString(dataReader.GetValue(3) as byte[]);

            int id = dataReader.GetInt32(0);


            Debug.Log("User: " + userName + ". Salt: " + salt + ". ID: " + id + ". Hash: " + hash + ".");
        }
    }

    /// <summary>
    /// Inserts a new User Account into the SQLite database.
    /// Uses SQL parameters, guid salts, and hash for basic security.
    /// </summary>
    /// <param name="newUsername">The user supplied name for the new account.</param>
    /// <param name="newPasscode">The user supplied passcode for the new account.</param>
    /// <param name="dbConnection">The database connection object for the database in use.</param>
    private void CreateAccount(string newUsername, string newPasscode, IDbConnection dbConnection)
    {
        //Generate Salt
        System.Guid guid = System.Guid.NewGuid();

        //Combine salt with newPasscode
        //ASCII works with db, unicode does not? Experiment.
        byte[] encodedPasscode = System.Text.Encoding.ASCII.GetBytes(newPasscode + guid + newUsername);

        //Hash Salted Passcode
        ///Create Hashgen

        SHA256 sHA256 = SHA256.Create();

        byte[] computedHash = sHA256.ComputeHash(encodedPasscode);

        string finalHash = ByteArrayContentsToString(computedHash);
      
        //Insert query to database - new entry in user account table
        ///Sends User and Hash
        ///Need Database connection to do this - Should have a class that saves connection. Scriptable object?

        string insertQuery = "INSERT into UserAccounts(username, hash, salt) VALUES(@newUsername, @finalHash, @guid);";
        IDbCommand dbCommand = dbConnection.CreateCommand();

        CreateNamedParamater("@newUsername", newUsername, dbCommand);
        CreateNamedParamater("@finalHash", finalHash, dbCommand);
        CreateNamedParamater("@guid", guid.ToString(), dbCommand);
        dbCommand.CommandText = insertQuery;
        
        dbCommand.ExecuteNonQuery();
        Debug.Log("Attempted Account creation");

        //DB side - Trigger after insert: Create new User data table
    }

    /// <summary>
    /// Utility: Quickly builds a new generic paramater and adds it to a specified IdbCommand class.
    /// </summary>
    /// <param name="parameterName">The desired SQL parameter name.</param> 
    /// <param name="parameterValue">The value to assign to the parameter.</param>
    /// <param name="dbCommand">The IDbCommand object this parameter is associated with.</param> 
    private void CreateNamedParamater(string parameterName, object parameterValue, IDbCommand dbCommand)
    {
        IDataParameter parameter = dbCommand.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.Value = parameterValue;
        dbCommand.Parameters.Add(parameter);
    }

    /// <summary>
    /// Attempts to verify a user account by testing provided information against information in the database,
    /// using hash comparisons. Currently reads out debug logs in unity for results.
    /// </summary>
    /// <param name="username">A user provided account name.</param>
    /// <param name="passcode">A user provided passcode for the account.</param>
    /// <param name="dbConnection">The database connection object for the database in use.</param>
    /// <returns>True if the information provided matches the database information, false if it does not.</returns>
    private bool VerifyAccount(string username, string passcode, IDbConnection dbConnection)
    {
        SHA256 sHA256 = SHA256.Create();
        string selectQuerySaltHash = "SELECT salt, hash FROM UserAccounts WHERE username = @username;";
        IDbCommand dbCommand = dbConnection.CreateCommand();
        CreateNamedParamater("@username", username, dbCommand);
        dbCommand.CommandText = selectQuerySaltHash;
        IDataReader dataReader = dbCommand.ExecuteReader();
        string salt = "";
        string hash = "";

        while (dataReader.Read())
        {
            //salt = ByteArrayToString(dataReader.GetValue(0) as byte[]);
            //hash = ByteArrayToString(dataReader.GetValue(1) as byte[]);
            byte[] salttemp = dataReader.GetValue(0) as byte[];
            byte[] hashtemp = dataReader.GetValue(1) as byte[];


            salt = System.Text.Encoding.ASCII.GetString(salttemp);
            hash = System.Text.Encoding.ASCII.GetString(hashtemp);
            
            //Debug.Log("TestHold");
        }
        dataReader.Close();

        byte[] encodedPasscode = System.Text.Encoding.ASCII.GetBytes(passcode + salt + username);
        byte[] computedHash = sHA256.ComputeHash(encodedPasscode);

        if(hash == ByteArrayContentsToString(computedHash))
        {
            Debug.Log("Salt from db is:" + salt);
            Debug.Log("Hash From db is: " + hash);
            Debug.Log("Generated Hash is: " + ByteArrayContentsToString(computedHash));
            return true;
        }
        else
        {
            Debug.Log("Salt from db is: " + salt);
            Debug.Log("Hash From db is: " + hash);
            Debug.Log("Generated Hash is: " + ByteArrayContentsToString(computedHash));
            return false;
        }
    }

    private void GrantAuth(bool verified, string username, IDbConnection dbConnection)
    {
        if (verified)
        {
            string selectQueryID = "SELECT ID FROM UserAccounts WHERE username = @username;";
            IDbCommand dbCommand = dbConnection.CreateCommand();
            CreateNamedParamater("@username", username, dbCommand);
            dbCommand.CommandText = selectQueryID;
            IDataReader dataReader = dbCommand.ExecuteReader();
            //Temp assigned -1 to prevent data collision
            int tempID = -1;

            while (dataReader.Read())
            {
                tempID = dataReader.GetInt32(0);
            }
            dataReader.Close();

            //Create "AuthToken"
            UserSessionSingleton.CreateUserSessionInstance(tempID, username);

        }
    }
    
    /// <summary>
    /// Utility: Bulids a string from the contents of a byte[].
    /// Reasoning: byte[].ToString returns " 'System.Byte[]' " as opposed to a string of the contents of the array.
    /// </summary>
    /// <param name="hash">A byte array representing a hash value.</param>
    /// <returns></returns>
    string ByteArrayContentsToString(byte[] hash)
    {
        string hashString = "";

        foreach (byte x in hash)
        {
            hashString += x.ToString();
        }

        return hashString;
    }

}
