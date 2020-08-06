using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// A class which contains and updates player statistics.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    AchieveLogic _achieveLogic;
    StatsConnection _statsConnection;
    PlayerStatBlock _playerStatBlock;
    PlayerAchievementBlock _playerAchievementBlock;

    #region Stat Getters
    public int GetTotalGathers()
    {
        return _playerStatBlock.totalGatheringPointsHarvested;
    }
    public float GetTotalDistanceTraveled()
    {
        return _playerStatBlock.totalDistanceTraveled;
    }
    public int GetTotalGold()
    {
        return _playerStatBlock.totalGoldCollected;
    }

    #endregion

    #region Stat Updaters

    public async Task UpdateGatheringPointsTotal(int amount)
    {
        _playerStatBlock.totalGatheringPointsHarvested += amount;
        if(_achieveLogic.CheckUnlockStatus(EAchievements.TotalGathers))
        {
            _playerAchievementBlock.totalGathersUnlocked = true;
        }
        await Task.Run(() => _statsConnection.AsyncUpdatePlayerStat(EUserStats.nodesHarvested, _playerStatBlock));
        
    }

    public void UpdateDistanceTotal(float amount)
    {
        _playerStatBlock.totalDistanceTraveled += amount;
        if (_achieveLogic.CheckUnlockStatus(EAchievements.DistanceTraveled))
        {
            _playerAchievementBlock.totalDistanceUnlocked = true;
        }
    }
    public async Task UpdateGoldTotal(int amount)
    {
        _playerStatBlock.totalGoldCollected += amount;
        Debug.Log("Total Gold: " + _playerStatBlock.totalGoldCollected);
        if (!_playerAchievementBlock.totalGoldUnlocked && _achieveLogic.CheckUnlockStatus(EAchievements.TotalGold))
        {
            _playerAchievementBlock.totalGoldUnlocked = true;
            Debug.Log("Collected 100 gold! " + _playerAchievementBlock.totalGoldUnlocked + " " + _playerStatBlock.totalGoldCollected);
            await Task.Run(() => _statsConnection.AsyncUpdatePlayerAchievementUnlock(EAchievements.TotalGold, _playerAchievementBlock.totalGoldUnlocked));
        }
        await Task.Run( () => _statsConnection.AsyncUpdatePlayerStat(EUserStats.goldEarned, _playerStatBlock));
    }

    #endregion

    #region Achievement Getters

    public bool IsTotalGathersUnlocked()
    {
        return _playerAchievementBlock.totalGathersUnlocked;
    }

    public bool IsTotalDistanceTraveledUnlocked()
    {
        return _playerAchievementBlock.totalDistanceUnlocked;
    }

    public bool IsTotalGoldUnlocked()
    {
        return _playerAchievementBlock.totalGoldUnlocked;
    }

    #endregion

    public void StatInit()
    {

    }


    // Start is called before the first frame update
    void Start()
    {
        _achieveLogic = new AchieveLogic(this);
        _statsConnection = GameObject.FindObjectOfType<StatsConnection>();
        PlayerStatBlock statBlock = _statsConnection.GetUserStatsFromDB();

        _playerStatBlock.totalGatheringPointsHarvested = statBlock.totalGatheringPointsHarvested;
        _playerStatBlock.totalDistanceTraveled = statBlock.totalDistanceTraveled;
        _playerStatBlock.totalGoldCollected = statBlock.totalGoldCollected;

        PlayerAchievementBlock achievementBlock = _statsConnection.GetUserAchievementsFromDB();

        _playerAchievementBlock.totalGathersUnlocked = achievementBlock.totalGathersUnlocked;
        _playerAchievementBlock.totalDistanceUnlocked = achievementBlock.totalDistanceUnlocked;
        _playerAchievementBlock.totalGoldUnlocked = achievementBlock.totalGoldUnlocked;

        //foreach(bool isUnlocked in _achieveLogic.CheckUnlockStatusAll())
        //{
            
        //}


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
