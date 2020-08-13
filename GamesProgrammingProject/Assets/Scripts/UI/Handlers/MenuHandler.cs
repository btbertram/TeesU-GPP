using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// A class used to react to Unity UI events and hide,
/// show, or otherwise navigate though different UI menus.
/// </summary>
public class MenuHandler : MonoBehaviour
{

    GameObject prevCanvas;
    GameObject messageCanvas;
    GameObject loadingCanvas;
    GameObject loginCanvas;
    GameObject registrationCanvas;
    GameObject mainMenuCanvas;
    GameObject pauseCanvas;
    GameObject leaderboardCanvas;
    GameObject achievementCanvas;
    GameObject focusedMenu;
    GameObject focusedSubMenu;
    Text messageCanvasText;

    #region CANVAS_GETTERS
    public GameObject GetMessageCanvas()
    {
        return messageCanvas;
    }

    public GameObject GetLoadingCanvas()
    {
        return loadingCanvas;
    }

    public GameObject GetMainMenuCanvas()
    {
        return mainMenuCanvas;
    }

    public GameObject GetPauseCanvas()
    {
        return pauseCanvas;
    }

    public GameObject GetFocusedMenu()
    {
        return focusedMenu;
    }

    public GameObject GetFocusedSubMenu()
    {
        return focusedSubMenu;
    }

    public GameObject GetLoginCanvas()
    {
        return loginCanvas;
    }

    public GameObject GetRegistrationCanvas()
    {
        return registrationCanvas;
    }

    public GameObject GetAchievementCanvas()
    {
        return achievementCanvas;
    }

    public GameObject GetLeaderboardCanvas()
    {
        return leaderboardCanvas;
    }

    #endregion

    public void ToggleCanvas(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    public void ToggleButtonInteractable(Button button)
    {
        button.interactable = !button.interactable;
    }

    public void SetPrevCanvas(GameObject gameObject)
    {
        prevCanvas = gameObject;
    }

    public void ClearPrevCanvas()
    {
        prevCanvas = null;
    }

    public void RestorePrevCanvas()
    {
        prevCanvas.SetActive(true);
        prevCanvas = null;
    }

    public void UpdateConfirmationMessageText(string labelText, bool success)
    {
        if (success)
        {
            labelText += " Success";
        }
        else
        {
            labelText += " Failure";
        }

        messageCanvasText.text = labelText;
    }

    public void ClickQuit()
    {
        GameManager.GetGMInstance().UnityApplicationQuit();
    }

    public void ClickUnpause()
    {
        //GameManager.GetGMInstance().UnimplementedTimescaleStopFunction()
    }

    public void ClickLoadSceneOvrWorld()
    {
        //Loads Overworld Scene
        //LoadSceneAsync would be better, but it's not actually an async function (it's yield), and a loading screen is a "should have"
        //so I'll skip that for now.
        SceneManager.LoadScene(1);
    }
    public void SetMenuFocus(GameObject gameObject)
    {
        focusedMenu = gameObject;
    }

    public void ClearMenuFocus()
    {
        focusedMenu = null;
    }

    public void SetSubMenuFocus(GameObject gameObject)
    {
        focusedSubMenu = gameObject;
    }

    public void ClearSubMenuFocus()
    {
        focusedSubMenu = null;
    }

    /// <summary>
    /// When clicking upon this active button, causes this button to become inactive, and causes inactive buttons this button
    /// shares a parent with to become active.
    /// Reacts to OnClick, set in editor.
    /// </summary>
    /// <param name="button">The button being clicked upon.</param>
    public void ChangeActiveButton(Button button)
    {
        var buttons = button.gameObject.transform.parent.GetComponentsInChildren<Button>();

        foreach(Button x in buttons)
        {
            if (!x.interactable)
            {
                ToggleButtonInteractable(x);
            }
        }
        ToggleButtonInteractable(button);

    }

    // Start is called before the first frame update
    void Start()
    {
        //init state for UI

        //This implementation should be faster than Find by name
        Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
        
        foreach(Canvas canvas in canvases)
        {
            canvas.gameObject.SetActive(false);

            switch (canvas.name)
            {
                case nameof(ECanvasNames.LoginCanvas):
                    canvas.gameObject.SetActive(true);
                    loginCanvas = canvas.gameObject;
                    break;
                case nameof(ECanvasNames.RegistrationCanvas):
                    registrationCanvas = canvas.gameObject;
                    break;
                case nameof(ECanvasNames.ConfirmationMessageCanvas):
                    messageCanvas = canvas.gameObject;                                     
                    Text[] textholder = canvas.gameObject.GetComponentsInChildren<Text>();
                    foreach (Text x in textholder)
                    {
                        if (x.text == "Confirm Message Placeholder")
                        {
                            messageCanvasText = x;
                        }
                    }
                    break;
                case nameof(ECanvasNames.LoadingCanvas):
                    loadingCanvas = canvas.gameObject;
                    break;
                case nameof(ECanvasNames.MainMenuCanvas):
                    mainMenuCanvas = canvas.gameObject;
                    break;
                case nameof(ECanvasNames.PauseCanvas):
                    pauseCanvas = canvas.gameObject;
                    break;
                case nameof(ECanvasNames.AchievementCanvas):
                    achievementCanvas = canvas.gameObject;
                    break;
                case nameof(ECanvasNames.LeaderboardCanvas):
                    leaderboardCanvas = canvas.gameObject;
                    break;
                default:
                    break;
            }
        }
    }
}
