using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class DisplayStatsConnection : MonoBehaviour
{

    public (string, string) GetAchievementInfoFromDB(EAchievements eAchievements)
    {
        IDbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        string achieveName = "";
        string achieveDesc = "";
        string selectQuery = "SELECT name, description FROM Achievements WHERE achievementID = @aID;";
        ConnectionManager.CreateNamedParamater("@aID", (int)eAchievements, dbCommand);
        dbCommand.CommandText = selectQuery;
        IDataReader reader = dbCommand.ExecuteReader();

        while (reader.Read())
        {
            achieveName = reader.GetString(0);
            achieveDesc = reader.GetString(1);
        }

        reader.Close();
        reader.Dispose();
        dbCommand.Dispose();

        return (achieveName, achieveDesc);
    }

    public (float, float) GetPlayerUnlockInfoFromDB(EAchievements achievement)
    {
        float totalPlayers = -1;
        float unlockedPlayers = -1;


        IDbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        //Get total # of players with achievement logs
        string selectQuery = "SELECT count(DISTINCT(playerID)) FROM PlayerAchievements;";
        dbCommand.CommandText = selectQuery;

        IDataReader reader = dbCommand.ExecuteReader();
        while (reader.Read())
        {
            totalPlayers = reader.GetInt64(0);
        }
        reader.Close();
        reader.Dispose();


        //Get total # of players with this achievement unlocked
        selectQuery = "SELECT count(playerID) FROM PlayerAchievements WHERE achievementID = @aID AND unlocked = 1;";
        ConnectionManager.CreateNamedParamater("@aID", (int)achievement, dbCommand);

        dbCommand.CommandText = selectQuery;

        IDataReader reader2 = dbCommand.ExecuteReader();
        while (reader2.Read())
        {
            unlockedPlayers = reader2.GetInt64(0);
        }
        reader2.Close();
        reader2.Dispose();
        dbCommand.Dispose();

        return (unlockedPlayers, totalPlayers);
    }

    public void QueryRankingStat(EUserStats userStat, List<string> usernames, IList statList)
    {
        ConnectionManager.GetCMInstance();
        IDbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        //Variables don't seem to work with the formating for this query.
        string selectQuery = "SELECT UserStats." + userStat.ToString() + ", UserAccounts.username FROM UserStats INNER JOIN UserAccounts ON UserStats.UserID = UserAccounts.ID ORDER BY " + userStat.ToString() + " desc;";
        dbCommand.CommandText = selectQuery;
        IDataReader reader = dbCommand.ExecuteReader();
        while (reader.Read())
        {
            switch (userStat)
            {
                case EUserStats.distanceTraveled:
                    statList.Add(reader.GetFloat(0));
                    break;
                case EUserStats.goldEarned:
                    statList.Add(reader.GetInt32(0));
                    break;
                case EUserStats.nodesHarvested:
                    statList.Add(reader.GetInt32(0));
                    break;
                default:
                    break;
            }
            usernames.Add(reader.GetString(1));
        }
        reader.Close();
        reader.Dispose();
        dbCommand.Dispose();
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
