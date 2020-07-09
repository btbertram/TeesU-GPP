using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class used to react to Unity UI events and hide,
/// show, or otherwise navigate though different UI menus.
/// </summary>
public class MenuHandler : MonoBehaviour
{

    GameObject prevCanvas;
    public GameObject messageCanvas;
    public GameObject loadingCanvas;
    Text messageCanvasText;

    public void ToggleCanvas(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    public void ToggleCanvasSetPrev(GameObject gameObject)
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
        prevCanvas = gameObject;
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
                case "Registration Canvas":
                    canvas.gameObject.SetActive(false);
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
                default:
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
