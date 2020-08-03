using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which contains and updates player statistics.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    AchieveLogic _achieveLogic;

    int totalGoldCollected;
    int totalGatheringPointsHarvested;
    float totalDistanceTraveled;

    bool achievementTotalGathersUnlocked;
    bool achievementDistanceTraveledUnlocked;
    bool achievementTotalGoldUnlocked;

    #region Stat Getters
    public int GetTotalGold()
    {
        return totalGoldCollected;
    }

    public int GetTotalGathers()
    {
        return totalGatheringPointsHarvested;
    }

    public float GetTotalDistanceTravelled()
    {
        return totalDistanceTraveled;
    }

    #endregion

    #region Stat Updaters

    public void UpdateGoldTotal(int amount)
    {
        totalGoldCollected += amount;
        Debug.Log("Total Gold: " + totalGoldCollected);
        if (_achieveLogic.CheckUnlockStatus(EAchievements.TotalGold))
        {
            achievementTotalGoldUnlocked = true;
            Debug.Log("Collected 100 gold! " + achievementTotalGoldUnlocked + " " + totalGoldCollected);
        }
    }

    public void UpdateGatheringPointsTotal(int amount)
    {
        totalGatheringPointsHarvested += amount;
        if(_achieveLogic.CheckUnlockStatus(EAchievements.TotalGathers))
        {
            achievementTotalGathersUnlocked = true;
        }
    }

    public void UpdateDistanceTotal(float amount)
    {
        totalDistanceTraveled += amount;
        if (_achieveLogic.CheckUnlockStatus(EAchievements.DistanceTraveled))
        {
            achievementDistanceTraveledUnlocked = true;
        }
    }

    #endregion

    #region Achievement Getters

    public bool IsTotalGathersUnlocked()
    {
        return achievementTotalGathersUnlocked;
    }

    public bool IsTotalDistanceTravelledUnlocked()
    {
        return achievementDistanceTraveledUnlocked;
    }

    public bool IsTotalGoldUnlocked()
    {
        return achievementTotalGoldUnlocked;
    }

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        _achieveLogic = new AchieveLogic(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
