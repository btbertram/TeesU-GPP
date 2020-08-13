using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
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
    List<string> usernames = new List<string>();
    IList statList;

    public DisplayStatsConnection DisplayStatsConnection;

    GameObject LoadAddLeaderRowToContent()
    {
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rectTransform.rect.height + leaderboardDisplayBoxSize);
        GameObject newLeaderBox = GameObject.Instantiate(leaderboardDisplayBoxPrefab, this.transform);

        return newLeaderBox;
    }

    void UtilityClearStatLists()
    {

        statList.Clear();
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
                    case nameof(ELeaderboardBoxTexts.LeaderboardRankingLabel):
                        currentText.text = (rankLoopCounter+1).ToString();
                        break;
                    case nameof(ELeaderboardBoxTexts.LeaderboardPlayerNameLabel):
                        currentText.text = usernames[rankLoopCounter];
                        break;
                    case nameof(ELeaderboardBoxTexts.LeaderboardStatValueLabel):
                        currentText.text = statList[rankLoopCounter].ToString();
                        break;
                    default:
                        break;
                }
            }

            rankLoopCounter += 1;
        }
    }

    void Awake()
    {
        rectTransform = this.gameObject.GetComponent<RectTransform>();
        DisplayStatsConnection = FindObjectOfType<DisplayStatsConnection>();
    }

    async Task LeaderBoardContentSetup()
    {
        switch (eStat)
        {
            case EUserStats.distanceTraveled:
                statList = new List<float>();
                break;
            case EUserStats.goldEarned:
                statList = new List<int>();
                break;
            case EUserStats.nodesHarvested:
                statList = new List<int>();
                break;
            default:
                break;
        }

        UtilityClearStatLists();
        await Task.Run(() => DisplayStatsConnection.QueryRankingStatAsync(eStat, usernames, statList));
        
        //Optimization issue:
        //Large collection here for these two functions, causes the game to hang/frame drop while instanciating and assigning text.
        foreach (string entry in usernames)
        {
            LeaderRowBoxes.Add(LoadAddLeaderRowToContent());
        }
        AssignLeaderDisplayBoxTexts(eStat);
    }

    public async Task RefreshBoard()
    {
        foreach(GameObject box in LeaderRowBoxes)
        {
            Destroy(box);
        }
        LeaderRowBoxes.Clear();
        LeaderRowBoxes.TrimExcess();
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

        await LeaderBoardContentSetup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
