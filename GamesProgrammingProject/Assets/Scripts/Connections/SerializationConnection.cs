using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;

public class SerializationConnection : MonoBehaviour
{


    public async Task SavePlayerStatus()
    {
        var playerObject = GameObject.Find("Player");
        var playerPos = playerObject.GetComponent<Transform>().position;
        var playerGold = playerObject.GetComponent<PlayerData>().GetGoldHeld();

        string updateQuery = "UPDATE PlayerStatus SET posX = @posX, posY = @posY, posZ = @posZ, goldCount = @currentGold WHERE playerID = @id;";

        IDbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        ConnectionManager.CreateNamedParamater("@posX", playerPos.x, dbCommand);
        ConnectionManager.CreateNamedParamater("@posY", playerPos.y, dbCommand);
        ConnectionManager.CreateNamedParamater("@posZ", playerPos.z, dbCommand);
        ConnectionManager.CreateNamedParamater("@currentGold", playerGold, dbCommand);
        ConnectionManager.CreateNamedParamater("id", UserSessionManager.GetID(), dbCommand);

        dbCommand.CommandText = updateQuery;

        await Task.FromResult(dbCommand.ExecuteNonQuery());

    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
