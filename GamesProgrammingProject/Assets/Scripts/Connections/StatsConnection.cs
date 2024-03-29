﻿using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A Connection Class which communicates with the database to update or query for player statistics,
/// such as distance traveled, or the total amount of gold they've collected.
/// Also handles achievements, since they're a subset of stats.
/// </summary>
public class StatsConnection : MonoBehaviour
{

    PlayerStats _playerStats;

    public async Task<PlayerStatBlock> GetUserStatsFromDBAsync()
    {
        //ConnectionManager.OpenInstanceConnection();

        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();

        string selectQuery = "SELECT nodesHarvested, distanceTraveled, goldEarned FROM UserStats WHERE UserID = @ID;";
        ConnectionManager.CreateNamedParamater("@ID", UserSessionManager.GetID(), dbCommand);

        dbCommand.CommandText = selectQuery;
        Task<DbDataReader> readerTask = dbCommand.ExecuteReaderAsync();
        
        PlayerStatBlock statBlock = new PlayerStatBlock();


        DbDataReader reader = await readerTask;

        while (reader.Read())
        {
            statBlock.totalGatheringPointsHarvested = reader.GetInt32(0);
            statBlock.totalDistanceTraveled = reader.GetFloat(1);
            statBlock.totalGoldCollected = reader.GetInt32(2);
        }
        reader.Close();
        reader.Dispose();
        dbCommand.Dispose();

        //ConnectionManager.CloseInstanceConnection();

        return statBlock;
    }

    public async Task<PlayerAchievementBlock> GetUserAchievementsFromDBAsync()
    {

        PlayerAchievementBlock achievementBlock = new PlayerAchievementBlock();

        //ConnectionManager.OpenInstanceConnection();
        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();

        string selectQuery = "SELECT achievementID, unlocked FROM PlayerAchievements WHERE playerID = @ID;";
        ConnectionManager.CreateNamedParamater("@ID", UserSessionManager.GetID(), dbCommand);

        dbCommand.CommandText = selectQuery;

        Task<DbDataReader> readerTask = dbCommand.ExecuteReaderAsync();
        DbDataReader reader = await readerTask;

        while (reader.Read())
        {
            bool isUnlocked = false;
            int achieveID = reader.GetInt32(0);
            int unlockedint = reader.GetInt32(1);
            //Bool is stored as int in SQLite, convert
            if(unlockedint == 1)
            {
                isUnlocked = true;
            }

            switch (achieveID)
            {
                case (int)EAchievements.TotalGathers:
                    achievementBlock.totalGathersUnlocked = isUnlocked;
                    break;
                case (int)EAchievements.DistanceTraveled:
                    achievementBlock.totalDistanceUnlocked = isUnlocked;
                    break;
                case (int)EAchievements.TotalGold:
                    achievementBlock.totalGoldUnlocked = isUnlocked;
                    break;
            }
        }
        reader.Close();
        reader.Dispose();
        dbCommand.Dispose();

        //ConnectionManager.CloseInstanceConnection();

        return achievementBlock;
    }

    public async Task UpdatePlayerStatsAllAsync(PlayerStatBlock statBlock)
    {

        //ConnectionManager.OpenInstanceConnection();
        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        string updateQuery = "UPDATE UserStats SET nodesHarvested = @nodesHarvested, distanceTraveled = @distanceTraveled, goldEarned = @goldEarned WHERE userID = @ID;";

        ConnectionManager.CreateNamedParamater("@nodesHarvested", statBlock.totalGatheringPointsHarvested, dbCommand);
        ConnectionManager.CreateNamedParamater("@distanceTraveled", statBlock.totalDistanceTraveled, dbCommand);
        ConnectionManager.CreateNamedParamater("@goldEarned", statBlock.totalGoldCollected, dbCommand);
        ConnectionManager.CreateNamedParamater("@ID", UserSessionManager.GetID(), dbCommand);
        
        dbCommand.CommandText = updateQuery;

        await Task.Run(() => dbCommand.ExecuteNonQuery());

        dbCommand.Dispose();
        //ConnectionManager.CloseInstanceConnection();
    }

    public async Task UpdatePlayerStatAsync(EUserStats userStat, PlayerStatBlock statBlock)
    {
        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        string updateQuery = "UPDATE UserStats SET " + userStat.ToString() + " = @" + userStat.ToString() + " WHERE userID = @ID;";
        ConnectionManager.CreateNamedParamater("@ID", UserSessionManager.GetID(), dbCommand);
        switch (userStat)
        {
            case EUserStats.distanceTraveled:
                ConnectionManager.CreateNamedParamater("@" + userStat.ToString(), statBlock.totalDistanceTraveled, dbCommand);
                break;
            case EUserStats.goldEarned:
                ConnectionManager.CreateNamedParamater("@" + userStat.ToString(), statBlock.totalGoldCollected, dbCommand);
                break;
            case EUserStats.nodesHarvested:
                ConnectionManager.CreateNamedParamater("@" + userStat.ToString(), statBlock.totalGatheringPointsHarvested, dbCommand);
                break;
            default:
                break;
        }


        dbCommand.CommandText = updateQuery;
        await Task.Run(() => dbCommand.ExecuteNonQuery());
        dbCommand.Dispose();

    }

    public async Task UpdatePlayerAchievementUnlockAsync(EAchievements achievement, bool unlocked)
    {
        //ConnectionManager.OpenInstanceConnection();
        DbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        Debug.Log("Unlock Attempt");
        Debug.Log((int)achievement);
        int dbbool = 0;
        if (unlocked)
        {
            dbbool = 1;
        }
        
        string updateQuery = "UPDATE PlayerAchievements SET unlocked = @unlocked WHERE playerID = @uID AND achievementID = @aID;";
        ConnectionManager.CreateNamedParamater("@unlocked", dbbool, dbCommand);
        ConnectionManager.CreateNamedParamater("@uID", UserSessionManager.GetID(), dbCommand);
        ConnectionManager.CreateNamedParamater("@aID", (int)achievement, dbCommand);

        dbCommand.CommandText = updateQuery;
        await Task.Run(() => dbCommand.ExecuteNonQuery());
        dbCommand.Dispose();
        //ConnectionManager.CloseInstanceConnection();
    }

}
