using SuperPupSystems.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public Health health;

    void Start()
    {

    }

    
    void Update()
    {
        
    }

    public void OnHealthChanged()
    {
        // other on healing things
    }

    public void Healed()
    {
        // Play a healing vfx
    }

    public void Hurt()
    {
        // Play a hurt vfx/sounds
    }

    public void OutOfHealth()
    {
        // Respawn and reset things here
    }
}
