using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Drawing;

/// <summary>
/// A script to automatically populate the database table "GatheringPoints" based on gathering points placed in the editor/designer.
/// I'd rather spend time automating this than making 50 queries prone to error due to manual entry. This also helps me practice more code anyway.
/// </summary>
public class GPointInsert : MonoBehaviour
{

    GatheringPoint[] gatheringPoints;

    // Start is called before the first frame update
    void Start()
    {
        gatheringPoints = GameObject.FindObjectsOfType<GatheringPoint>();
        ClearGatheringPointsDatabaseTable();
        PopulateGatheringPointDatabaseTable(gatheringPoints);
    }


    private async void AsyncClearGatheringPointsDatabaseTable()
    {
        Debug.Log("Reached Async Clear");
        await new Task( () => ClearGatheringPointsDatabaseTable() );
    }

    private void ClearGatheringPointsDatabaseTable()
    {
        Debug.Log("Reached Clear");
        ConnectionManager.OpenInstanceConnection();
        IDbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        
        string deleteAllRecordsFromGatheringPoints = "DELETE FROM GatheringPoints;";
        dbCommand.CommandText = deleteAllRecordsFromGatheringPoints;
        dbCommand.ExecuteNonQuery();
        
        ConnectionManager.CloseInstanceConnection();
    }

    private async void AsyncPopulateGatheringPointDatabaseTable(GatheringPoint[] points)
    {
        await new Task(() => PopulateGatheringPointDatabaseTable(points));
    }

    private void PopulateGatheringPointDatabaseTable(GatheringPoint[] points)
    {
        ConnectionManager.GetCMInstance();
        ConnectionManager.OpenInstanceConnection();
        IDbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
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

        ConnectionManager.CloseInstanceConnection();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
