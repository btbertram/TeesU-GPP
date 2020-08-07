using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private MenuHandler _mHandler;
    GameObject focusedInteractable;
    private InteractionCollision _interactCollision;

    public void Pause()
    {
        if (Input.GetButtonDown(EInput.Cancel.ToString()))
        {
            if(_mHandler.GetFocusedMenu() != null)
            {
                if(_mHandler.GetFocusedSubMenu() != null)
                {
                    _mHandler.ToggleCanvas(_mHandler.GetFocusedSubMenu());
                    _mHandler.ClearSubMenuFocus();
                }
                _mHandler.ToggleCanvas(_mHandler.GetFocusedMenu());
                _mHandler.ClearMenuFocus();
                _mHandler.RestorePrevCanvas();
            }
            else
            {
                _mHandler.ToggleCanvas(_mHandler.GetPauseCanvas());
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
