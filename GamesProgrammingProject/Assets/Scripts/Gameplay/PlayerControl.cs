using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private MenuHandler _mHandler;
    //Set this in Editor
    public GameObject pauseMenu;
    GameObject focusedMenu;

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
    }

    // Update is called once per frame
    void Update()
    {
        Pause();
    }
}
