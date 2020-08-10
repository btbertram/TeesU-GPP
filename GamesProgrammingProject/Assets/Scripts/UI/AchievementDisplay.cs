using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDisplay : MonoBehaviour
{
    RectTransform rectTransform;
    public GameObject AchievementDisplayBoxPrefab;
    public float achievementDisplayBoxSize = 200;
    List<GameObject> AchievementBoxes = new List<GameObject>();
    public bool isPlayerAchieve = true;

    GameObject LoadAddAchievementToContent(GameObject AchievementDisplayBox, EAchievements eAchievements)
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height + achievementDisplayBoxSize);
        var newAchieveBox = GameObject.Instantiate(AchievementDisplayBox, this.transform);
        ConnectionManager.GetCMInstance();
        
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

        var textarray = newAchieveBox.GetComponentsInChildren<Text>();

        foreach(Text text in textarray)
        {
            switch (text.name)
            {                
                case nameof(EAchievementBoxTexts.AchievementNameLabel):
                    text.text = achieveName;
                    break;
                case nameof(EAchievementBoxTexts.AchievementDescriptionLabel):
                    text.text = achieveDesc;
                    break;
                default:
                    break;
            }
        }
        return newAchieveBox;

    }

    public void UpdateUIAchievementStatus()
    {
        if (isPlayerAchieve)
        {
            var achievementBlock = GameObject.FindObjectOfType<PlayerStats>().GetPlayerAchievementBlock();

            SetBoxUnlocked(achievementBlock.totalDistanceUnlocked, EAchievements.DistanceTraveled);
            SetBoxUnlocked(achievementBlock.totalGathersUnlocked, EAchievements.TotalGathers);
            SetBoxUnlocked(achievementBlock.totalGoldUnlocked, EAchievements.TotalGold);

            var statBlock = GameObject.FindObjectOfType<PlayerStats>().GetPlayerStatBlock();

            SetAchievementProgress(achievementBlock.totalDistanceUnlocked, statBlock.totalDistanceTraveled, EAchievements.DistanceTraveled);
            SetAchievementProgress(achievementBlock.totalGathersUnlocked, statBlock.totalGatheringPointsHarvested, EAchievements.TotalGathers);
            SetAchievementProgress(achievementBlock.totalGoldUnlocked, statBlock.totalGoldCollected, EAchievements.TotalGold);

        }
        else
        {
            SetPercentPlayersUnlocked(EAchievements.DistanceTraveled);
            SetPercentPlayersUnlocked(EAchievements.TotalGathers);
            SetPercentPlayersUnlocked(EAchievements.TotalGold);
        }
    }


    void SetPercentPlayersUnlocked(EAchievements achievement)
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

        float percentUnlocked = unlockedPlayers / totalPlayers;
        AchievementBoxes[(int)achievement].GetComponentInChildren<Slider>().value = percentUnlocked;
        var texts = AchievementBoxes[(int)achievement].GetComponentsInChildren<Text>();

        foreach (Text x in texts)
        {
            if (x.name == "AchievementProgressLabel")
            {
                x.text = percentUnlocked * 100 + "% of players have this achievement.";
            }
        }

    }

    void SetAchievementProgress(bool unlocked, float progress, EAchievements achievement)
    {
        float achievementGoal = -1;
        switch (achievement)
        {
            case EAchievements.DistanceTraveled:
                achievementGoal = 50;
                break;
            case EAchievements.TotalGathers:
                achievementGoal = 5;
                break;
            case EAchievements.TotalGold:
                achievementGoal = 100;
                break;
            default:
                break;
        }

        if (unlocked)
        {
            progress = achievementGoal;
        }

        float newVal = Mathf.Clamp(progress / achievementGoal, 0, 1);

        AchievementBoxes[(int)achievement].GetComponentInChildren<Slider>().value = newVal;
        var texts = AchievementBoxes[(int)achievement].GetComponentsInChildren<Text>();

        foreach(Text x in texts)
        {
            if(x.name == "AchievementProgressLabel")
            {
                x.text = progress + "/" + achievementGoal;
            }
        }
    }

    public void SetBoxUnlocked(bool unlocked, EAchievements achievement)
    {
        Debug.Log("ToggleUnlockUI");
        if (unlocked)
        {
            AchievementBoxes[(int)achievement].GetComponent<Image>().color = Color.green;
        }
        else
        {
            AchievementBoxes[(int)achievement].GetComponent<Image>().color = Color.red;
        }
    }
    void Awake()
    {
        rectTransform = this.gameObject.GetComponent<RectTransform>();

        for(int x = 0; x < (int)EAchievements.Error; x++)
        {
            GameObject box = LoadAddAchievementToContent(AchievementDisplayBoxPrefab, (EAchievements)x);
            AchievementBoxes.Insert(x, box);
        }

    }

    void OnEnable()
    {
        UpdateUIAchievementStatus();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
