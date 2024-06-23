using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    private PlayerInputActions playerControls;
    private InputAction pause;

    public GameObject pauseMenuGobj;

    private void Awake()
    {
        playerControls = new PlayerInputActions();

        // turn the pause menu off by default
        if (pauseMenuGobj.activeSelf)
        {
            pauseMenuGobj.SetActive(false);
        }
    }

    void OnEnable()
    {
        pause = playerControls.Player.Pause;
        pause.Enable();

        pause.performed += PerformPause;
    }

    private void PerformPause(InputAction.CallbackContext context)
    {
        pauseMenuGobj.SetActive(!pauseMenuGobj.activeSelf);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resume()
    {
        pauseMenuGobj.SetActive(false);
    }


}
