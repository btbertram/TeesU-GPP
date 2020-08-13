using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class used to react to Unity UI events regarding changing and loading in
/// content for UI elements.
/// </summary>
public class ContentHandler : MonoBehaviour
{
    GameObject LeaderboardCanvasObject;
    GameObject AchievementCanvasObject;
    GameObject[] _leaderboardContents;
    GameObject[] _achievementContents;

    public void LeaderboardHeaderStatValueTextChange(GameObject ButtonText)
    {
        var texts = GameObject.FindGameObjectWithTag("HeaderLeaderboard").GetComponentsInChildren<Text>();
        foreach (Text currentText in texts)
        {
            switch (currentText.name)
            {
                case nameof(ELeaderboardBoxTexts.LeaderboardStatValueLabel):
                    currentText.text = ButtonText.GetComponentInChildren<Text>().text;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// An event function which sets the GameObject's parent's ScrollRect active content to be the GameObject.
    /// </summary>
    /// <param name="newContent">The Content to be displayed in the scroll rect viewport.</param>
    public void SetParentScrollRectContentAs(GameObject newContent)
    {
        //Get the object with the scrollrect on it: ScrollView -> Viewport -> Content
        //content.transform.parent = viewport -> .transform.parent = scrollview.
        newContent.transform.parent.transform.parent.GetComponent<ScrollRect>().content = newContent.GetComponent<RectTransform>();
    }

    public async void ClickSetActiveContent(GameObject gameObject)
    {
        GameObject[] contents = null;
        switch (gameObject.tag)
        {
            case nameof(ETags.LeaderboardContent):
                contents = _leaderboardContents;
                break;
            case nameof(ETags.AchievementContent):
                contents = _achievementContents;
                break;
        }

        foreach (GameObject x in contents)
        {
            if (x.activeInHierarchy)
            {
                x.SetActive(false);
            }
            else if(x == gameObject)
            {
                x.SetActive(true);
            }
        }

        SetParentScrollRectContentAs(gameObject);

        switch (gameObject.tag)
        {
            case nameof(ETags.LeaderboardContent):
                await gameObject.GetComponent<LeaderboardDisplay>().RefreshBoardAsync();
                break;
            case nameof(ETags.AchievementContent):
                await gameObject.GetComponent<AchievementDisplay>().UpdateUIAchievementStatusAsync();
                break;
            default:
                break;
        }


    }

    /// <summary>
    /// Ensures that the default view is displayed when navigating to the Leaderboard menu.
    /// Reacts to OnClick, set in editor.
    /// </summary>
    public void LeaderboardCanvasSetup()
    {
        var buttons = LeaderboardCanvasObject.GetComponentsInChildren<Button>();
        foreach(GameObject x in _leaderboardContents)
        {
            if(x.name == "LeaderboardContentGold")
            {
                x.SetActive(true);
            }
            else
            {
                x.SetActive(false);
            }
        }

        foreach(Button x in buttons)
        {
            if(x.name == "GoldRankingButton")
            {
                x.interactable = false;
            }
            else
            {
                x.interactable = true;
            }
        }

    }


    public void AchievementCanvasSetup()
    {
        var buttons = AchievementCanvasObject.GetComponentsInChildren<Button>();
        foreach (GameObject x in _achievementContents)
        {
            if (x.name == "PlayerAchievementContent")
            {
                x.SetActive(true);
            }
            else
            {
                x.SetActive(false);
            }
        }

        foreach (Button x in buttons)
        {
            if (x.name == "PlayerAchievementsButton")
            {
                x.interactable = false;
            }
            else
            {
                x.interactable = true;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LeaderboardCanvasObject = GameObject.FindObjectOfType<MenuHandler>().GetLeaderboardCanvas();
        AchievementCanvasObject = GameObject.FindObjectOfType<MenuHandler>().GetAchievementCanvas();
    }

    void Awake()
    {
        //These are here to gain references to them BEFORE MenuHandler.cs sets UI elements to active = false,
        //As you cannot search for inactive objects by tag
        _leaderboardContents = GameObject.FindGameObjectsWithTag(ETags.LeaderboardContent.ToString());
        _achievementContents = GameObject.FindGameObjectsWithTag(ETags.AchievementContent.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
