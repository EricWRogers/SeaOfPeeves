using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public Transform respawnPoint;

    public PlayerMovment player;

    public int scoreToWin;

    private int currentScore = 0; 

    public void RegisterPlayer(PlayerMovment _player)
    {
        player = _player;   
    }

    public void Respawn()
    {
        if(player != null)
        {
            player.transform.position = respawnPoint.position;
        }
    }
}
