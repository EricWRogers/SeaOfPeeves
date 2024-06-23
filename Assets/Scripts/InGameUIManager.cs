using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameUIManager : SingletonMonoBehaviour<InGameUIManager>
{
    public GameObject pausePanel;

    private bool paused;

    private PlayerInputActions playerControls;
    private InputAction pause;

    public TextMeshProUGUI scoreUI;
    public GameObject winScreen;

    new void Awake()
    {
        base.Awake();

        playerControls = new PlayerInputActions();

        winScreen.SetActive(false);
        pausePanel.SetActive(false);
        paused = false;
    }

    void OnEnable()
    {
        pause = playerControls.Player.Pause;
        pause.Enable();

        pause.performed += PauseInput;
    }

    public void PauseInput(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        paused = !paused;
        pausePanel.SetActive(paused);

        Time.timeScale = paused ? 0 : 1;
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
