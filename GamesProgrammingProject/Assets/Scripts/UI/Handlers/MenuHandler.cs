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
    //These three GameObjects are used by the Connection Handler
    GameObject messageCanvas;
    GameObject loadingCanvas;
    GameObject mainMenuCanvas;
    Text messageCanvasText;

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

    public void ToggleCanvas(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
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

    // Start is called before the first frame update
    void Start()
    {
        //init state for UI

        //This implementation should be faster than Find by name
        Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
        
        //Note: Add enum system or look into tags to make this less error prone
        foreach(Canvas canvas in canvases)
        {
            switch (canvas.name)
            {
                case "Login Canvas":
                    canvas.gameObject.SetActive(true);
                    break;
                case "Confirmation Message Canvas":
                    messageCanvas = canvas.gameObject;                                     
                    Text[] textholder = canvas.gameObject.GetComponentsInChildren<Text>();
                    foreach (Text x in textholder)
                    {
                        if (x.text == "Confirm Message Placeholder")
                        {
                            messageCanvasText = x;
                        }
                    }
                    canvas.gameObject.SetActive(false);
                    break;
                case "Loading Canvas":
                    loadingCanvas = canvas.gameObject;
                    canvas.gameObject.SetActive(false);
                    break;
                case "Main Menu Canvas":
                    mainMenuCanvas = canvas.gameObject;
                    canvas.gameObject.SetActive(false);
                    break;
                default:
                    canvas.gameObject.SetActive(false);
                    break;
            }
        }
    }
}
