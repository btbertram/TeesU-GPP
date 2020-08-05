﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private MenuHandler _mHandler;
    //Set this in Editor
    public GameObject pauseMenu;
    GameObject focusedMenu;
    GameObject focusedInteractable;
    private InteractionCollision _interactCollision;

    public void Pause()
    {
        if (Input.GetButtonDown(EInput.Cancel.ToString()))
        {
            if(focusedMenu != null)
            {
                _mHandler.ToggleCanvas(focusedMenu);
                ClearMenuFocus();
            }
            else
            {
                _mHandler.ToggleCanvas(pauseMenu);
                //Also toggle time here, call from GameManager
            }
        }
    }


    public void Interact()
    {
        if (Input.GetButtonDown(EInput.Use.ToString()) && _interactCollision.CanInteract())
        {
            Debug.Log("Attempting Interact");

            var interactable = _interactCollision.GetFirstInteractableFromCollisionCollection();

            interactable.InteractionTriggered();
            _interactCollision.ToggleCanInteract();
            _interactCollision.interacters.Remove(_interactCollision.GetFirstInteractableFromCollisionCollection());

        }
    }

    public void SetMenuFocus(GameObject gameObject)
    {
        focusedMenu = gameObject;
    }

    public void ClearMenuFocus()
    {
        focusedMenu = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        _mHandler = GameObject.FindObjectOfType<MenuHandler>();
        _interactCollision = GameObject.FindObjectOfType<InteractionCollision>();

    }

    // Update is called once per frame
    void Update()
    {
        Pause();
        Interact();
        DEBUGTestUserInfoInput();
        DEBUGTestDBConnectionInput();
    }

    void DEBUGTestUserInfoInput()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log(UserSessionManager.GetUsername());
            Debug.Log(UserSessionManager.GetID());

        }
    }

    void DEBUGTestDBConnectionInput()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log(ConnectionManager.GetConnection().State);
        }
    }
}
