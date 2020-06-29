using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.Security.Cryptography;
using System.Threading;
using System.Dynamic;
using System.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

public sealed class ConnectionManager
{
    //Check OpenInstanceConnection for string definition.
    private string _internalConnectionString;
    private IDbConnection _dbConnection;

    //Simple return type structure, used for providing information to UI windows.
    public struct BoolStringResult
    {
        public bool _successful;
        public string _stringMessage;
    }

    #region Singleton Implementation
    private static ConnectionManager _connectionManagerInstance;

    private static readonly object _lock = new object();

    private ConnectionManager()
    {

    }

    public static ConnectionManager GetCMInstance()
    {
        if(_connectionManagerInstance == null)
        {
            lock (_lock)
            {
                if(_connectionManagerInstance == null)
                {
                    _connectionManagerInstance = new ConnectionManager();
                }
            }
        }
        return _connectionManagerInstance;
    }
    #endregion


    #region Utility Functions

    /// <summary>
    /// Prints text debug logs in Unity to test fetching and displaying information from the SQLite database.
    /// </summary>
    /// <param name="dbConnection">The Database connection object for the database in use.</param> 
    void DebugCode()
    {
        string selectQuery = "SELECT * FROM TestItem";
        IDbCommand dbCommand = _dbConnection.CreateCommand();
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
    /// Opens a new connection to the database for the ConnectionManager instance.
    /// </summary>
    /// <remarks>
    /// Always use CloseInstanceConnection() when finished. Best practice is to use them like braces.
    /// </remarks>
    public static void OpenInstanceConnection()
    {
        _connectionManagerInstance._internalConnectionString = "URI=file:" + Application.dataPath + "/GameDB.db";

        _connectionManagerInstance._dbConnection = new SqliteConnection(_connectionManagerInstance._internalConnectionString);

        _connectionManagerInstance._dbConnection.Open();
    }

    public static void CloseInstanceConnection()
    {
        _connectionManagerInstance._dbConnection.Close();
        _connectionManagerInstance._dbConnection = null;
    }

    /// <summary>
    /// Utility: Quickly builds a new generic paramater and adds it to a specified IdbCommand class.
    /// </summary>
    /// <param name="parameterName">The desired SQL parameter name.</param> 
    /// <param name="parameterValue">The value to assign to the parameter.</param>
    /// <param name="dbCommand">The IDbCommand object this parameter is associated with.</param> 
    private static void CreateNamedParamater(string parameterName, object parameterValue, IDbCommand dbCommand)
    {
        IDataParameter parameter = dbCommand.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.Value = parameterValue;
        dbCommand.Parameters.Add(parameter);
    }

    /// <summary>
    /// Utility: Bulids a string from the contents of a byte[].
    /// Reasoning: byte[].ToString returns " 'System.Byte[]' " as opposed to a string of the contents of the array.
    /// </summary>
    /// <param name="hash">A byte array representing a hash value.</param>
    /// <returns></returns>
    private static string ByteArrayContentsToString(byte[] hash)
    {
        string hashString = "";

        foreach (byte x in hash)
        {
            hashString += x.ToString();
        }

        return hashString;
    }

    #endregion

    #region Async Wrappers
    public static async Task<BoolStringResult> CreateAccountAsync(string username, string passcode)
    {
        //Debug: Wait for two seconds
        await Task.Delay(2000);

        var result = await CreateAccount(username, passcode);

        return result;
    }

    public static async Task<BoolStringResult> VerifyAccountAsync(string username, string passcode)
    {
        //Debug: Wait for five seconds
        await Task.Delay(2000);
        var result = await Task.FromResult<BoolStringResult>(VerifyAccount(username, passcode));

        return result;
    }


    #endregion

    private static BoolStringResult InputQuickExit(string username, string passcode)
    {
        BoolStringResult result;

        if (username == "")
        {
            result._successful = false;
            result._stringMessage = "No Username Given. Please enter a username.";

            return result;
        }

        if (passcode == "")
        {
            result._successful = false;
            result._stringMessage = "No Password Given. Please enter a password.";

            return result;
        }

        result._stringMessage = "";
        result._successful = true;
        return result;
    }

    /// <summary>
    /// Inserts a new User Account into the SQLite database.
    /// Uses SQL parameters, guid salts, and hash for basic security.
    /// </summary>
    /// <param name="newUsername">The user supplied name for the new account.</param>
    /// <param name="newPasscode">The user supplied passcode for the new account.</param>
    /// <param name="dbConnection">The database connection object for the database in use.</param>
    public static async Task<BoolStringResult> CreateAccount(string newUsername, string newPasscode)
    {
        BoolStringResult result;

        //Test username
        result = InputQuickExit(newUsername, newPasscode);
        if (!result._successful)
        {
            return result;
        }

        result = await Task.FromResult<BoolStringResult>(TestUsernameAvailability(newUsername));
        if (!result._successful)
        {
            return result;            
        }

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
        IDbCommand dbCommand = _connectionManagerInstance._dbConnection.CreateCommand();

        CreateNamedParamater("@newUsername", newUsername, dbCommand);
        CreateNamedParamater("@finalHash", finalHash, dbCommand);
        CreateNamedParamater("@guid", guid.ToString(), dbCommand);
        dbCommand.CommandText = insertQuery;
        
        ///ExecuteNonQuery returns # of rows affected by command when-
        ///command is a UPDATE, INSERT, or DELETE.
        ///-1 is returned when any other command is used.
        int returnVal = dbCommand.ExecuteNonQuery();
        Debug.Log("Attempted Account creation");
        if(returnVal == 1)
        {
            //Check if newly created account works
            result = await VerifyAccountAsync(newUsername, newPasscode);
            if (result._successful)
            {
                result._stringMessage = "Account Created!";
                return result;
            }
            else
            {
                result._stringMessage = "Unable to verify account creation.";
                return result;
            }
                
        }
        else
        {
            result._successful = false;
            result._stringMessage = "Error during account creation.";
            return result;
        }
        //DB side - Trigger after insert: Create new User data table
    }

    private static BoolStringResult TestUsernameAvailability(string username)
    {
        BoolStringResult result;

        IDbCommand dbCommand = _connectionManagerInstance._dbConnection.CreateCommand();
        CreateNamedParamater("@username", username, dbCommand);

        string query = "SELECT username FROM UserAccounts WHERE username = @username;";
        dbCommand.CommandText = query;

        IDataReader dataReader = dbCommand.ExecuteReader();

        if (dataReader.Read())
        {
            //if there's anything to read, we have a matching username, so...
            result._stringMessage = "Username in use.";
            result._successful = false;
            return result;
        }
        else
        {
            //Otherwise, there are no results, which mean no matches for that username.
            //The provided username is available, so...
            result._stringMessage = "";
            result._successful = true;
            return result;
        }
    }

    /// <summary>
    /// Attempts to verify a user account by testing provided information against information in the database,
    /// using hash comparisons. Currently reads out debug logs in unity for results.
    /// </summary>
    /// <param name="username">A user provided account name.</param>
    /// <param name="passcode">A user provided passcode for the account.</param>
    /// <param name="dbConnection">The database connection object for the database in use.</param>
    /// <returns>True if the information provided matches the database information, false if it does not.</returns>
    public static BoolStringResult VerifyAccount(string username, string passcode)
    {
        BoolStringResult result;

        result = InputQuickExit(username, passcode);
        if (!result._successful)
        {
            return result;
        }

        
        SHA256 sHA256 = SHA256.Create();
        string selectQuerySaltHash = "SELECT salt, hash FROM UserAccounts WHERE username = @username;";
        IDbCommand dbCommand = _connectionManagerInstance._dbConnection.CreateCommand();
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

        if (hash == ByteArrayContentsToString(computedHash))
        {
            Debug.Log("Salt from db is:" + salt);
            Debug.Log("Hash From db is: " + hash);
            Debug.Log("Generated Hash is: " + ByteArrayContentsToString(computedHash));
            result._successful = true;
            result._stringMessage = "";
            return result;
        }
        else
        {
            Debug.Log("Salt from db is: " + salt);
            Debug.Log("Hash From db is: " + hash);
            Debug.Log("Generated Hash is: " + ByteArrayContentsToString(computedHash));
            result._successful = false;
            result._stringMessage = "Invalid Username or Password.";
            return result;
        
        }
    }

    public static void GrantAuth(bool verified, string username)
    {
        if (verified)
        {
            string selectQueryID = "SELECT ID FROM UserAccounts WHERE username = @username;";
            IDbCommand dbCommand = _connectionManagerInstance._dbConnection.CreateCommand();
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
            UserSessionManager.CreateUserSessionInstance(tempID, username);

        }
    }
    


}
