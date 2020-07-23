using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A Class that implents database connections and queries for GatheringPoint objects.
/// </summary>
public class GatheringConnection : MonoBehaviour
{
    public GameObject GoldGatherPointPrefab;


    public async Task AsyncLoadGatheringPoints()
    {
        await new Task(() => { LoadGatheringPoints(); });
    }

    private void LoadGatheringPoints()
    {

        Debug.Log("Loading Points...");
        string selectGatheringPoints = "SELECT * FROM GatheringPoints;";

        ConnectionManager.OpenInstanceConnection();

        IDbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        dbCommand.CommandText = selectGatheringPoints;

        List<GameObject> gatheringPointObjects = new List<GameObject>();
        int pointID;
        EGatherPointType type;
        float posX;
        float posY;
        float posZ;
        int loopcounter = -1;
        //Create a gameobject with the component, then set things on the component.

        IDataReader reader = dbCommand.ExecuteReader();
        while (reader.Read())
        {
            loopcounter += 1;

            GameObject newgp;
            GatheringPoint gpscript;
            Quaternion zeroQuaternion = new Quaternion(0,0,0,0);
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
                    break;

                default:
                    break;
            }

        }
        reader.Close();
        reader.Dispose();
        dbCommand.Dispose();
        ConnectionManager.CloseInstanceConnection();
        Debug.Log("Points Loaded");
    }

    public async Task<long> AsyncQueryGatherTime(int gatherPointID)
    {

        return await Task.FromResult(QueryGatherTime(gatherPointID));

    }

    private long QueryGatherTime(int gatherPointID)
    {
        ConnectionManager.OpenInstanceConnection();

        IDbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        string selectQueryTimeGathered = "SELECT timeHarvested FROM GatheringPoints WHERE pointID = @ID;";
        ConnectionManager.CreateNamedParamater("@ID", gatherPointID, dbCommand);
        dbCommand.CommandText = selectQueryTimeGathered;
        IDataReader reader = dbCommand.ExecuteReader();
        long time = -1;

        while (reader.Read())
        {
            time = reader.GetInt64(0);
        }
        reader.Close();
        reader.Dispose();
        dbCommand.Dispose();

        ConnectionManager.CloseInstanceConnection();
        return time;

    }

    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.GetCMInstance();
        LoadGatheringPoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
