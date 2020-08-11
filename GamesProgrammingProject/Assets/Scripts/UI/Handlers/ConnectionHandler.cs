using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A class used to react to Unity UI events in regards to connections.
/// </summary>
public class ConnectionHandler : MonoBehaviour
{

    private string _username;
    private string _passcode;
    private MenuHandler _mHandler;
    private AccountConnection _aConnection;
    private SerializationConnection _sConnection;

    //To be later used for telling user if their name contains invalid characters, etc.
    public void UpdateUsernameField()
    {
        
    }

    public void UpdatePasscodeField()
    {

    }

    public async void ClickVerify()
    {
        _mHandler = GameObject.FindObjectOfType<MenuHandler>();
        _aConnection = GameObject.FindObjectOfType<AccountConnection>();
        InputField codeInputField = null;
        InputField nameInputField = null;

        var test = GameObject.FindObjectsOfType<InputField>();
        foreach(InputField x in test)
        {
            switch (x.name)
            {
                case nameof(EInputFieldNames.UsernameInputField):
                    nameInputField = x;
                    break;

                case nameof(EInputFieldNames.PasswordInputField):
                    codeInputField = x;
                    break;

            }
        }

        try
        {
            _username = nameInputField.text;
            _passcode = codeInputField.text;
        }
        catch (NullReferenceException)
        {
            throw new NullReferenceException("ClickVerify: Use of unassigned InputField.");
        }

        var result = await _aConnection.VerifyAccountAsync(_username, _passcode);
        Debug.Log(result._successful);
        _aConnection.GrantAuth(result._successful, _username);

        Debug.Log(UserSessionManager.GetUsername());
        Debug.Log(UserSessionManager.GetID());

        _mHandler.UpdateConfirmationMessageText(result._stringMessage + " Login", result._successful);
        if (result._successful)
        {
            _mHandler.SetPrevCanvas(_mHandler.GetMainMenuCanvas());
        }
        _mHandler.ToggleCanvas(_mHandler.GetLoadingCanvas());
        _mHandler.ToggleCanvas(_mHandler.GetMessageCanvas());


    }

    public async void ClickRegister()
    {
        _mHandler = GameObject.FindObjectOfType<MenuHandler>();
        _aConnection = GameObject.FindObjectOfType<AccountConnection>();

        InputField codeInputField = null;
        InputField nameInputField = null;

        var test = GameObject.FindObjectsOfType<InputField>();
        foreach (InputField x in test)
        {
            switch (x.name)
            {
                case nameof(EInputFieldNames.NewUsernameInputField):
                    nameInputField = x;
                    break;

                case nameof(EInputFieldNames.NewPasswordInputField):
                    codeInputField = x;
                    break;

            }
        }

        try
        {
            _username = nameInputField.text;
            _passcode = codeInputField.text;
        }
        catch (NullReferenceException)
        {
            throw new NullReferenceException("ClickRegister: Use of unassigned InputField.");
        }

        var result = await _aConnection.CreateAccountAsync(_username, _passcode);

        _mHandler.UpdateConfirmationMessageText(result._stringMessage + " Registration", result._successful);
        _mHandler.ToggleCanvas(_mHandler.GetLoadingCanvas());
        _mHandler.ToggleCanvas(_mHandler.GetMessageCanvas());

    }

    public async void ClickSave()
    {
        _sConnection = GameObject.FindObjectOfType<SerializationConnection>();

        await _sConnection.AsyncSaveFullPlayerStatus();

    }

    // Start is called before the first frame update
    void Start()
    {
        ConnectionManager.GetCMInstance();
    }

    // Update is called once per frame
    void Update()
    {
        
    }




}
