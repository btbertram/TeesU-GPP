using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.Threading;
using System.Dynamic;
using System.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;

public sealed class ConnectionManager
{
    private readonly string _internalConnectionString = "URI=file:" + Application.dataPath + "/GameDB.db";
    public readonly IDbConnection _dbConnection;

    #region Singleton Implementation
    private static ConnectionManager _connectionManagerInstance;

    private static readonly object _lock = new object();

    private ConnectionManager()
    {
        _dbConnection = new SqliteConnection(_internalConnectionString);
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


    #region Utility

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
        _connectionManagerInstance._dbConnection.Open();
    }

    public static void CloseInstanceConnection()
    {
        _connectionManagerInstance._dbConnection.Close();
    }

    public static IDbConnection GetConnection()
    {
        try
        {
            return _connectionManagerInstance._dbConnection;
        }
        catch (NullReferenceException)
        {
            Debug.LogError("Error: Instance has no constucted connection.");
            return null;
        }
    }

    /// <summary>
    /// Utility: Quickly builds a new generic paramater and adds it to a specified IdbCommand class.
    /// </summary>
    /// <param name="parameterName">The desired SQL parameter name.</param> 
    /// <param name="parameterValue">The value to assign to the parameter.</param>
    /// <param name="dbCommand">The IDbCommand object this parameter is associated with.</param> 
    public static void CreateNamedParamater(string parameterName, object parameterValue, IDbCommand dbCommand)
    {
        IDataParameter parameter = dbCommand.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.Value = parameterValue;
        dbCommand.Parameters.Add(parameter);
    }

    public static async Task<long> AsyncQueryTimeNow()
    {
        return await Task.FromResult(QueryCurrentTime());
    }

    /// <summary>
    /// Queries the database for the current time.
    /// </summary>
    /// <returns>The current time according to the database, or -1 if no data could be selected.</returns>
    private static long QueryCurrentTime()
    {
        OpenInstanceConnection();
        IDbCommand dbCommand = GetConnection().CreateCommand();
        string selectQueryTimeNow = "SELECT strftime('%s','now')";
        dbCommand.CommandText = selectQueryTimeNow;
        IDataReader dataReader = dbCommand.ExecuteReader();

        long time = -1;
        while (dataReader.Read())
        {
            time = dataReader.GetInt64(0);
        }

        CloseInstanceConnection();
        return time;
    }

    /// <summary>
    /// Utility: Bulids a string from the contents of a byte[].
    /// Reasoning: byte[].ToString returns " 'System.Byte[]' " as opposed to a string of the contents of the array.
    /// </summary>
    /// <param name="hash">A byte array representing a hash value.</param>
    /// <returns></returns>
    public static string ByteArrayContentsToString(byte[] hash)
    {
        string hashString = "";

        foreach (byte x in hash)
        {
            hashString += x.ToString();
        }

        return hashString;
    }

    #endregion



   
    


}
