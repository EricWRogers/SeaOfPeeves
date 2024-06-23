using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HarpoonGun : MonoBehaviour
{
    public GameObject harpoon;
    public Transform firePoint;
    private GameObject shot;
    private PlayerInputActions playerControls;
    private InputAction fire;
    public bool fired;
    // Start is called before the first frame update
    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        fire = playerControls.Player.Fire;
        fire.Enable();
    }
    private void OnDisable()
    {
        fire.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if(!fired && playerControls.Player.Fire.triggered) { 
            fired = true;
            shot = GameObject.Instantiate(harpoon, firePoint.position, transform.rotation);
            shot.GetComponent<Harpoon>().returnPoint = firePoint;
            shot.GetComponent<Harpoon>().ret = false;
        }
        else if(fired && playerControls.Player.Fire.triggered) {
            shot.GetComponent<Harpoon>().ret = true;
        }
    }
}
