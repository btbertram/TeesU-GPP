using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A Connection Class that implents database connections and queries for GatheringPoint objects.
/// </summary>
public class GatheringConnection : MonoBehaviour
{
    public GameObject GoldGatherPointPrefab;

    public void LoadGatheringPointsAsync()
    {
        string selectGatheringPoints = "SELECT * FROM GatheringPoints;";

        //ConnectionManager.OpenInstanceConnection();

        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        dbCommand.CommandText = selectGatheringPoints;
        Task<DbDataReader> readerTask = dbCommand.ExecuteReaderAsync();

        List<GameObject> gatheringPointObjects = new List<GameObject>();
        int pointID;
        EGatherPointType type;
        float posX;
        float posY;
        float posZ;
        int loopcounter = -1;
        GameObject newgp;
        GatheringPoint gpscript;
        Quaternion zeroQuaternion = new Quaternion(0,0,0,0);
        //Create a gameobject with the component, then set things on the component.

        DbDataReader reader = readerTask.Result;
        while (reader.Read())
        {
            loopcounter += 1;

            pointID = reader.GetInt32(0);
            type = (EGatherPointType)reader.GetInt32(1);
            posX = reader.GetFloat(2);
            posY = reader.GetFloat(3);
            posZ = reader.GetFloat(4);

            var posLoad = new Vector3(posX, posY, posZ);

            switch (type)
            {
                case EGatherPointType.GoldGatherType :
                    newgp = GameObject.Instantiate(GoldGatherPointPrefab, posLoad, zeroQuaternion);
                    newgp.name = "gatheringPoint" + loopcounter;
                    gpscript = newgp.GetComponent<GatheringPoint>();
                    gpscript.LoadPoint(pointID, type);
                    newgp.gameObject.GetComponent<MeshRenderer>().enabled = false;
                    newgp.gameObject.GetComponent<BoxCollider>().enabled = false;
                    break;

                default:
                    break;
            }

        }
        reader.Close();
        reader.Dispose();
        dbCommand.Dispose();
        //ConnectionManager.CloseInstanceConnection();
    }

    public async Task<long> QueryGatherTimeAsync(int gatherPointID)
    {
        //ConnectionManager.OpenInstanceConnection();

        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        string selectQueryTimeGathered = "SELECT timeHarvested FROM GatheringPoints WHERE pointID = @ID;";
        ConnectionManager.CreateNamedParamater("@ID", gatherPointID, dbCommand);
        dbCommand.CommandText = selectQueryTimeGathered;
        Task<DbDataReader> readerTask = dbCommand.ExecuteReaderAsync();
        long time = -1;

        DbDataReader reader = await readerTask;
        while (reader.Read())
        {
            time = reader.GetInt64(0);
        }
        reader.Close();
        reader.Dispose();
        dbCommand.Dispose();

        //ConnectionManager.CloseInstanceConnection();
        return time;

    }


    public async Task AsyncRecordGatherTime(long currentTime, int gatherPointID)
    {
        //long currentTime = 1;
        //long currentTime = await ConnectionManager.AsyncQueryTimeNow();

        //ConnectionManager.OpenInstanceConnection();

        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        
        string updateQuery = "UPDATE GatheringPoints SET timeHarvested = @currentTime WHERE PointID = @ID;";
        ConnectionManager.CreateNamedParamater("@currentTime", currentTime, dbCommand);
        ConnectionManager.CreateNamedParamater("@ID", gatherPointID, dbCommand);

        dbCommand.CommandText = updateQuery;
        await Task.Run(() => dbCommand.ExecuteNonQueryAsync());

        dbCommand.Dispose();
        //return false;
        //ConnectionManager.CloseInstanceConnection();
    }


    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.GetCMInstance();
        LoadGatheringPointsAsync();
    }
}
