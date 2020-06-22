using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class used to react to Unity UI events in regards to connections.
/// </summary>
public class ConnectionHandler : MonoBehaviour
{

    private string _username;
    private string _passcode;

    //To be later used for telling user if their name contains invalid characters, etc.
    public void UpdateUsernameField()
    {
        
    }

    public void UpdatePasscodeField()
    {

    }

    public void ClickVerify()
    {
        InputField nameInputField = GameObject.Find("Username InputField").GetComponent<InputField>();
        InputField codeInputField = GameObject.Find("Password InputField").GetComponent<InputField>();

        _username = nameInputField.text;
        _passcode = codeInputField.text;

        Debug.Log(_username);
        Debug.Log(_passcode);

        ConnectionManager.OpenInstanceConnection();

        ConnectionManager.GrantAuth(ConnectionManager.VerifyAccount(_username, _passcode), _username);

        Debug.Log(UserSessionManager.GetUsername());
        Debug.Log(UserSessionManager.GetID());

        ConnectionManager.CloseInstanceConnection();
    }

    public void ClickRegister()
    {
        InputField nameInputField = GameObject.Find("New Username InputField").GetComponent<InputField>();
        InputField codeInputField = GameObject.Find("New Password InputField").GetComponent<InputField>();

        _username = nameInputField.text;
        _passcode = codeInputField.text;

        Debug.Log(_username);
        Debug.Log(_passcode);

        ConnectionManager.OpenInstanceConnection();

        ConnectionManager.CreateAccount(_username, _passcode);

        ConnectionManager.CloseInstanceConnection();

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }




}
