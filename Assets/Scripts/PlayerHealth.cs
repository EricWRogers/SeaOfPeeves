using SuperPupSystems.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Health health;

    public Slider healthSlider;

    void Start()
    {
        health.Revive();
    }

    
    void Update()
    {
        
    }

    public void OnHealthChanged()
    {
        Debug.Log("Updating health:" +  (float)health.currentHealth / (float)health.maxHealth);
        healthSlider.value = (float)health.currentHealth / (float)health.maxHealth;
    }

    public void Healed()
    {
        
    }

    public void Hurt()
    {
        
    }

    public void OutOfHealth()
    {
        // Respawn and reset things here
        health.Revive();
    }
}
