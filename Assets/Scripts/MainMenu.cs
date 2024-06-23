using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameplayScene;

    public GameObject creditObject;
    public GameObject splashObject;

    public void Start()
    {
        creditObject.SetActive(false);    
    }

    public void ToggleCredits()
    {
        creditObject.SetActive(!creditObject.activeSelf);
        splashObject.SetActive(!splashObject.activeSelf);
    }
  

    public void StartGame()
    {
        // Assumes 1 is the gameplay scene
        SceneManager.LoadScene(gameplayScene);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
