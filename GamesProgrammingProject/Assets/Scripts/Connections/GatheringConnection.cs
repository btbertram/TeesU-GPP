using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;

public class GatheringConnection : MonoBehaviour
{

    string _userSessionUser;
    int _userSessionID;
    IDbConnection connection;

    public async Task<long> AsyncQueryGatherTime(int gatherPointID)
    {

        return await Task.FromResult(QueryGatherTime(gatherPointID));

    }

    private long QueryGatherTime(int gatherPointID)
    {
        ConnectionManager.OpenInstanceConnection();

        IDbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        string selectQueryTimeGathered = "SELECT timeHarvested FROM GatheringPoints WHERE pointID = @ID";
        ConnectionManager.CreateNamedParamater("@ID", gatherPointID, dbCommand);
        dbCommand.CommandText = selectQueryTimeGathered;
        IDataReader dataReader = dbCommand.ExecuteReader();
        long time = -1;

        while (dataReader.Read())
        {
            time = dataReader.GetInt64(0);
        }

        ConnectionManager.CloseInstanceConnection();
        return time;

    }

    // Start is called before the first frame update
    void Start()
    {
        _userSessionUser = UserSessionManager.GetUsername();
        _userSessionID = UserSessionManager.GetID();
        ConnectionManager.GetCMInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
