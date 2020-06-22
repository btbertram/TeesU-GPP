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
    GameObject loginCanvas;
    GameObject registrationCanvas;
    GameObject messageCanvas;
    Text messageCanvasText;
    private bool showLoginCanvas;
    private bool showRegistrationCanvas;
    private bool showMessageCanvas;
    bool success;

    public void LoginCanvasToggle()
    {
        showLoginCanvas = !showLoginCanvas;
        loginCanvas.SetActive(showLoginCanvas);
    }
    public void RegistrationCanvasToggle()
    {
        showRegistrationCanvas = !showRegistrationCanvas;
        registrationCanvas.SetActive(showRegistrationCanvas);
    }

    public void MessageCanvasToggle()
    {
        showMessageCanvas = !showMessageCanvas;
        messageCanvas.SetActive(showMessageCanvas);
    }

    public void UpdateTextMessage(string labelText)
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
        showLoginCanvas = true;
        showRegistrationCanvas = false;

        loginCanvas = GameObject.Find("Login Canvas");
        registrationCanvas = GameObject.Find("Registration Canvas");
        messageCanvas = GameObject.Find("Confirmation Message Canvas");

        //The "right way to do it" accoring to unity documentation
        Text[] textholder = messageCanvas.GetComponents<Text>();
        foreach(Text x in textholder)
        {
            if (x.raycastTarget == false)
            {
                messageCanvasText = x;
            }
        }

        //The easy way to do it
        //messageCanvasText = messageCanvas.GetComponent("Message") as Text;


        loginCanvas.SetActive(showLoginCanvas);
        registrationCanvas.SetActive(showRegistrationCanvas);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
