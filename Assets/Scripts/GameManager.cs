using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public Transform respawnPoint;

    public PlayerMovment player;

    public float winDelay = 3f;
    public int scoreToWin;

    private int currentScore = 0;

    public string mainMenuScene;

    private void Start()
    {
        InGameUIManager.Instance.scoreUI.text = $"Score: {currentScore}/{scoreToWin}";
    }

    public void Respawn()
    {
        if(player != null)
        {
            player.transform.position = respawnPoint.position;
        }
    }

    public void IncrementScore(int _scoreToAdd)
    {
        currentScore += _scoreToAdd;
        InGameUIManager.Instance.scoreUI.text = $"Score: {currentScore}/{scoreToWin}";

        if(currentScore >= scoreToWin)
        {
            StartCoroutine(WinGame());
        }
    }

    public IEnumerator WinGame()
    {
        InGameUIManager.Instance.winScreen.SetActive(true);
        yield return new WaitForSeconds(winDelay);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(mainMenuScene);

    }
}
