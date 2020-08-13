using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that defines achievement unlocking behavior.
/// </summary>
public class AchieveLogic
{
    PlayerStats _playerStats;

    public AchieveLogic(PlayerStats playerStats)
    {
        _playerStats = playerStats;
    }


    public bool CheckUnlockStatus(EAchievements achievements)
    {
        switch (achievements)
        {

            case EAchievements.DistanceTraveled:
                if(_playerStats.GetTotalDistanceTraveled() >= 500 && !_playerStats.IsTotalDistanceTraveledUnlocked())
                {
                    return true;
                }
                return false;
                //break;

            case EAchievements.TotalGathers:

                if(_playerStats.GetTotalGathers() >= 5 && !_playerStats.IsTotalGathersUnlocked())
                {
                   return true;
                }
                return false;
                //break;

            case EAchievements.TotalGold:
                
                if(_playerStats.GetTotalGold() >= 100 && !_playerStats.IsTotalGoldUnlocked())
                {
                    return true;
                }
                return false;
                //break;

            default:
                throw new Exception("AchiveLogic.CheckUnlockStatus(): Reached Default Case.");
                //break;
        }

        throw new Exception("Error: Reached out of switch case in AchiveLogic.CheckUnlockStatus()");
    }
    
    /// <summary>
    /// Utility function to check all achievements.
    /// Can be called at game start to unlock "missed" or altered achievements. 
    /// </summary>
    //public IEnumerable<bool> CheckUnlockStatusAll()
    //{
    //    for (int x = 0; x < (int)EAchievements.Error; x++)
    //    {
    //        yield return CheckUnlockStatus((EAchievements)x);
    //    }
    //}

}
