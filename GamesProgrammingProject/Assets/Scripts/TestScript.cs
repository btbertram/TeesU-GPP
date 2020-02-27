using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

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
}
