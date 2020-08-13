using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Drawing;

/// <summary>
/// A designer tool script to automatically populate the database table "GatheringPoints" based on gathering points placed in the editor/designer.
/// To use, attach this to a gameobject, and press play in editor in the OverworldScene. Any GameObjects with the GatheringPoint.cs script
/// will be added as a gathering point to the database.
/// Turn off Gathering Connection and World Manager when running this script, or it'll double the inserts.
/// </summary>
public class GPointInsertConnection : MonoBehaviour
{

    GatheringPoint[] gatheringPoints;

    // Start is called before the first frame update
    void Start()
    {
        gatheringPoints = GameObject.FindObjectsOfType<GatheringPoint>();
        ClearGatheringPointsDatabaseTable();
        PopulateGatheringPointDatabaseTable(gatheringPoints);
    }

    private void ClearGatheringPointsDatabaseTable()
    {
        //ConnectionManager.OpenInstanceConnection();
        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        
        string deleteAllRecordsFromGatheringPoints = "DELETE FROM GatheringPoints;";
        dbCommand.CommandText = deleteAllRecordsFromGatheringPoints;
        dbCommand.ExecuteNonQuery();
        dbCommand.Dispose();

        //ConnectionManager.CloseInstanceConnection();
    }

    private void PopulateGatheringPointDatabaseTable(GatheringPoint[] points)
    {
        ConnectionManager.GetCMInstance();
        //ConnectionManager.OpenInstanceConnection();
        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        string insertGatheringPoint = "INSERT INTO GatheringPoints VALUES(@pointID, @gpType, @posX, @posY, @posZ, @goldVal, @timeHarvested);";
        int idCounter = -1;
        int goldVal = 0;
        //Set default time to 0, as a unix time, so we don't run into errors trying to manipulate null
        long timeHarvest = 0;

        foreach(GatheringPoint point in points)
        {

            switch (point.GetPointType())
            {
                case EGatherPointType.GoldGatherType:
                    {
                        goldVal = 10;
                        break;
                    }

                default:
                    {
                        break;
                    }
            }


            idCounter += 1;
            dbCommand.Parameters.Clear();
            ConnectionManager.CreateNamedParamater("@pointID", idCounter, dbCommand);
            ConnectionManager.CreateNamedParamater("@gpType", (int)point.GetPointType(), dbCommand);
            ConnectionManager.CreateNamedParamater("@posX", point.GetComponentInParent<Transform>().position.x, dbCommand);
            ConnectionManager.CreateNamedParamater("@posY", point.GetComponentInParent<Transform>().position.y, dbCommand);
            ConnectionManager.CreateNamedParamater("@posZ", point.GetComponentInParent<Transform>().position.z, dbCommand);
            ConnectionManager.CreateNamedParamater("@goldVal", goldVal, dbCommand);
            ConnectionManager.CreateNamedParamater("@timeHarvested", timeHarvest, dbCommand);
            dbCommand.CommandText = insertGatheringPoint;

            dbCommand.ExecuteNonQuery();

        }
            dbCommand.Dispose();

        //ConnectionManager.CloseInstanceConnection();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
