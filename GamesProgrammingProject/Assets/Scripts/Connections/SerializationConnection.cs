using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A connection class which communicates the player's data state, such as inventory, currency, etc. to the database.
/// </summary>
public class SerializationConnection : MonoBehaviour
{

    public GameObject playerObject;

    public async Task SaveFullPlayerStatusAsync()
    {

        var playerPos = playerObject.GetComponent<Transform>().position;
        var playerGold = playerObject.GetComponent<PlayerData>().GetGoldHeld();

        string updateQuery = "UPDATE PlayerStatus SET posX = @posX, posY = @posY, posZ = @posZ, goldCount = @currentGold WHERE playerID = @id;";

        //ConnectionManager.OpenInstanceConnection();
        
        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        ConnectionManager.CreateNamedParamater("@posX", playerPos.x, dbCommand);
        ConnectionManager.CreateNamedParamater("@posY", playerPos.y, dbCommand);
        ConnectionManager.CreateNamedParamater("@posZ", playerPos.z, dbCommand);
        ConnectionManager.CreateNamedParamater("@currentGold", playerGold, dbCommand);
        ConnectionManager.CreateNamedParamater("id", UserSessionManager.GetID(), dbCommand);

        dbCommand.CommandText = updateQuery;

        await Task.Run(() => dbCommand.ExecuteNonQuery());

        dbCommand.Dispose();
        
        //ConnectionManager.CloseInstanceConnection();

    }


    //public async Task AsyncSavePlayerGoldStatus()
    //{
    //    var playerGold = playerObject.GetComponent<PlayerData>().GetGoldHeld();

    //    string updateQuery = "UPDATE PlayerStatus SET goldCount = @currentGold WHERE playerID = @id;";

    //    //ConnectionManager.OpenInstanceConnection();

    //    DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
    //    ConnectionManager.CreateNamedParamater("@currentGold", playerGold, dbCommand);
    //    ConnectionManager.CreateNamedParamater("@id", UserSessionManager.GetID(), dbCommand);

    //    dbCommand.CommandText = updateQuery;

    //    await Task.FromResult(dbCommand.ExecuteNonQuery());

    //    dbCommand.Dispose();

    //    //ConnectionManager.CloseInstanceConnection();
    //}


    async public Task LoadPlayerStatusAsync()
    {
        //Default pos
        Vector3 loadPos = new Vector3(150, 1, 150);
        int loadGold = 0;
        string selectQuery = "SELECT * FROM PlayerStatus WHERE playerID = @id;";

        //ConnectionManager.OpenInstanceConnection();

        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        ConnectionManager.CreateNamedParamater("@id", UserSessionManager.GetID(), dbCommand);

        dbCommand.CommandText = selectQuery;

        Task<DbDataReader> readerTask = dbCommand.ExecuteReaderAsync();
        DbDataReader reader = await readerTask;

        while (reader.Read())
        {
            //Columns are: Id, posx, posy, posz, gold
            loadPos = new Vector3(reader.GetFloat(1), reader.GetFloat(2), reader.GetFloat(3));
            loadGold = reader.GetInt32(4);
        }
        reader.Close();
        reader.Dispose();
        dbCommand.Dispose();

        //ConnectionManager.CloseInstanceConnection();

        var pData = playerObject.GetComponent<PlayerData>();
        int initGoldVal = pData.GetGoldHeld();

        if(initGoldVal != 0)
        {
            pData.UpdatePlayerGold(-initGoldVal);
        }

        pData.UpdatePlayerGold(loadGold);
        playerObject.GetComponent<Transform>().position = loadPos;


    }

}
