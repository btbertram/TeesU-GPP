using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DisplayStatsConnection : MonoBehaviour
{

    public async Task<(string, string)> GetAchievementInfoFromDBAsync(EAchievements eAchievements)
    {
        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        string achieveName = "";
        string achieveDesc = "";
        string selectQuery = "SELECT name, description FROM Achievements WHERE achievementID = @aID;";
        ConnectionManager.CreateNamedParamater("@aID", (int)eAchievements, dbCommand);
        dbCommand.CommandText = selectQuery;
        Task<DbDataReader> readerTask = dbCommand.ExecuteReaderAsync();
        DbDataReader reader = await readerTask;

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

    public async Task<(float, float)> GetPlayerUnlockInfoFromDBAsync(EAchievements achievement)
    {
        float totalPlayers = -1;
        float unlockedPlayers = -1;


        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        //Get total # of players with achievement logs
        string selectQuery = "SELECT count(DISTINCT(playerID)) FROM PlayerAchievements;";
        dbCommand.CommandText = selectQuery;
        Task<DbDataReader> readerTask = dbCommand.ExecuteReaderAsync();

        DbDataReader reader = await readerTask;
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

        Task<DbDataReader> readerTask2 = dbCommand.ExecuteReaderAsync();
        DbDataReader reader2 = await readerTask2;
        while (reader2.Read())
        {
            unlockedPlayers = reader2.GetInt64(0);
        }
        reader2.Close();
        reader2.Dispose();
        dbCommand.Dispose();

        return (unlockedPlayers, totalPlayers);
    }

    public async Task QueryRankingStatAsync(EUserStats userStat, List<string> usernames, IList statList)
    {
        ConnectionManager.GetCMInstance();
        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        //Variables don't seem to work with the formating for this query.
        string selectQuery = "SELECT UserStats." + userStat.ToString() + ", UserAccounts.username FROM UserStats INNER JOIN UserAccounts ON UserStats.UserID = UserAccounts.ID ORDER BY " + userStat.ToString() + " desc;";
        dbCommand.CommandText = selectQuery;
        Task<DbDataReader> readerTask = dbCommand.ExecuteReaderAsync();
        DbDataReader reader = await readerTask;

        while (await reader.ReadAsync())
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

}
