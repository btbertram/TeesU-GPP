using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.Common;
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
    public readonly DbConnection _dbConnection;

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
                    OpenInstanceConnection();
                }
            }
        }
        return _connectionManagerInstance;
    }
    #endregion

    #region Utility

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

    public static DbConnection GetConnection()
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
    /// Utility: Quickly builds a new generic paramater and adds it to a specified DbCommand class.
    /// </summary>
    /// <param name="parameterName">The desired SQL parameter name.</param> 
    /// <param name="parameterValue">The value to assign to the parameter.</param>
    /// <param name="dbCommand">The DbCommand object this parameter is associated with.</param> 
    public static void CreateNamedParamater(string parameterName, object parameterValue, DbCommand dbCommand)
    {
        DbParameter parameter = dbCommand.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.Value = parameterValue;
        dbCommand.Parameters.Add(parameter);
    }

    /// <summary>
    /// Queries the database for the current time.
    /// </summary>
    /// <returns>The current time according to the database, or -1 if no data could be selected.</returns>
    public static async Task<long> QueryTimeNowAsync()
    {
        //OpenInstanceConnection();
        DbCommand dbCommand = GetConnection().CreateCommand();
        string selectQueryTimeNow = "SELECT strftime('%s','now');";
        dbCommand.CommandText = selectQueryTimeNow;

        Task<DbDataReader> readerTask = dbCommand.ExecuteReaderAsync();

        DbDataReader reader = await readerTask;

        long time = -1;
        while(reader.Read())
        {        
            //Raw value returned is as a "Time String". So, we must convert
            string timeString = reader.GetString(0);
            time = Int64.Parse(timeString);        
        }      
        
        reader.Close();
        reader.Dispose();
        dbCommand.Dispose();

        //CloseInstanceConnection();
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
