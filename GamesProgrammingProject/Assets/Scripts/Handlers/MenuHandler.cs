using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class used to react to Unity UI events and hide,
/// show, or otherwise navigate though different UI menus.
/// </summary>
public class MenuHandler : MonoBehaviour
{
    GameObject loginCanvas;
    GameObject registrationCanvas;
    private bool showLoginCanvas;
    private bool showRegistrationCanvas;

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


    // Start is called before the first frame update
    void Start()
    {
        showLoginCanvas = true;
        showRegistrationCanvas = false;

        loginCanvas = GameObject.Find("Login Canvas");
        registrationCanvas = GameObject.Find("Registration Canvas");

        loginCanvas.SetActive(showLoginCanvas);
        registrationCanvas.SetActive(showRegistrationCanvas);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
