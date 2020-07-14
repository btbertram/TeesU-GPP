using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using UnityEngine;

public class GatheringConnection : MonoBehaviour
{

    string _userSessionUser;
    int _userSessionID;
    IDbConnection connection;


    public async Task AsyncLoadGatheringPoints()
    {
        await new Task(() => LoadGatheringPoints());
    }

    private void LoadGatheringPoints()
    {
        ConnectionManager.OpenInstanceConnection();

        IDbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        string selectGatheringPoints = "SELECT * GatheringPoints";
        dbCommand.CommandText = selectGatheringPoints;
        IDataReader dataReader = dbCommand.ExecuteReader();

        List<GameObject> gatheringPointObjects = new List<GameObject>();
        int pointID;
        EGatherPointType type;
        float posX;
        float posY;
        float posZ;
        int loopcounter = -1;
        //Create a gameobject with the component, then set things on the component.

        while (dataReader.Read())
        {
            loopcounter += 1;
            var newgp = new GameObject("gatheringPoint" + loopcounter, typeof(GatheringPoint));
            var gpscript = newgp.GetComponent<GatheringPoint>();
            pointID = dataReader.GetInt32(0);
            type = (EGatherPointType)dataReader.GetInt32(1);
            posX = dataReader.GetFloat(2);
            posY = dataReader.GetFloat(3);
            posZ = dataReader.GetFloat(4);

            var posLoad = new Vector3(posX, posY, posZ);

            gpscript.LoadPoint(pointID, type, posLoad);

            gatheringPointObjects.Add(newgp);
        }


        ConnectionManager.CloseInstanceConnection();

        //Use information from the database to create a number of new game objects.
        //Each object will be assigned a matching id, selected from the database
        //That ID will be used to query for information from the database when necessary
        //Each object will be positioned, and assigned a type based on information in the database. 


    }

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
