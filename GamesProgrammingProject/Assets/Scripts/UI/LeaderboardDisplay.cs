using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardDisplay : MonoBehaviour
{

    RectTransform rectTransform;
    public GameObject leaderboardDisplayBoxPrefab;
    public float leaderboardDisplayBoxSize = 100;
    public EUserStats eStat;
    List<GameObject> LeaderRowBoxes = new List<GameObject>();
    List<int> goldStats = new List<int>();
    List<float> distanceStats = new List<float>();
    List<int> gatherStats = new List<int>();
    List<string> usernames = new List<string>();

    GameObject LoadAddLeaderRowToContent()
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height + leaderboardDisplayBoxSize);
        var newLeaderBox = GameObject.Instantiate(leaderboardDisplayBoxPrefab, this.transform);

        return newLeaderBox;
    }

    void QueryRankingStat(EUserStats userStat)
    {
        UtilityClearStatLists(userStat);
        ConnectionManager.GetCMInstance();
        IDbCommand dbCommand = ConnectionManager.GetConnection().CreateCommand();
        //Variables don't seem to work with the formating for this query.
        string selectQuery = "SELECT UserStats."+ userStat.ToString() + ", UserAccounts.username FROM UserStats INNER JOIN UserAccounts ON UserStats.UserID = UserAccounts.ID ORDER BY "+userStat.ToString()+" desc;";
        dbCommand.CommandText = selectQuery;
        Debug.Log(selectQuery);
        IDataReader reader = dbCommand.ExecuteReader();
        while (reader.Read())
        {
            switch (userStat)
            {
                case EUserStats.distanceTraveled:
                    distanceStats.Add(reader.GetFloat(0));
                    break;
                case EUserStats.goldEarned:
                    goldStats.Add(reader.GetInt32(0));
                    break;
                case EUserStats.nodesHarvested:
                    gatherStats.Add(reader.GetInt32(0));
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

    void UtilityClearStatLists(EUserStats userStat)
    {
        switch (userStat)
        {
            case EUserStats.distanceTraveled:
                distanceStats.Clear();
                break;
            case EUserStats.goldEarned:
                goldStats.Clear();
                break;
            case EUserStats.nodesHarvested:
                gatherStats.Clear();
                break;
            default:
                break;
        }
        usernames.Clear();
    }



    void AssignLeaderDisplayBoxTexts(EUserStats userStat)
    {
        int rankLoopCounter = 0;
        foreach(GameObject box in LeaderRowBoxes)
        {
            var texts = box.GetComponentsInChildren<Text>();
            foreach(Text currentText in texts)
            {
                switch (currentText.name)
                {
                    case "RankingLabel":
                        currentText.text = (rankLoopCounter+1).ToString();
                        break;
                    case "PlayerNameLabel":
                        currentText.text = usernames[rankLoopCounter];
                        break;
                    case "ValueLabel":
                        AssignValueLabelOnStatSwitch(userStat, currentText, rankLoopCounter);
                        break;
                    default:
                        break;
                }
            }

            rankLoopCounter += 1;
        }
    }

    void AssignValueLabelOnStatSwitch(EUserStats userStat, Text text, int index)
    {
        switch (userStat)
        {
            case EUserStats.distanceTraveled:
                text.text = distanceStats[index].ToString();
                break;
            case EUserStats.goldEarned:
                text.text = goldStats[index].ToString();
                break;
            case EUserStats.nodesHarvested:
                text.text = gatherStats[index].ToString();
                break;
            default:
                break;

        }
    }

    void Awake()
    {
        rectTransform = this.gameObject.GetComponent<RectTransform>();
        LeaderBoardContentSetup();
    }

    void LeaderBoardContentSetup()
    {
        QueryRankingStat(eStat);
        foreach (string entry in usernames)
        {
            LeaderRowBoxes.Add(LoadAddLeaderRowToContent());
        }
        AssignLeaderDisplayBoxTexts(eStat);
    }

    void RefreshBoard()
    {
        foreach(GameObject box in LeaderRowBoxes)
        {
            Destroy(box);
        }
        LeaderRowBoxes.Clear();
        LeaderRowBoxes.TrimExcess();
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

        LeaderBoardContentSetup();
    }

    void OnEnable()
    {
        RefreshBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
