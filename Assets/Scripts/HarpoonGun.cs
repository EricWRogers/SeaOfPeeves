using SuperPupSystems.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class HarpoonGun : MonoBehaviour
{
    public GameObject harpoon;
    public Transform firePoint;
    private GameObject shot;
    private PlayerInputActions playerControls;
    private InputAction fire;
    public bool fired;
    public int numPoints = 20;

    public LineRenderer lineRenderer;
    // Start is called before the first frame update
    private void Awake()
    {
        playerControls = new PlayerInputActions();
    }
    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numPoints;
        lineRenderer.enabled = false; 
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
            lineRenderer.enabled = true;
            shot = GameObject.Instantiate(harpoon, firePoint.position, transform.rotation);
            shot.GetComponent<Harpoon>().returnPoint = firePoint;
            shot.GetComponent<Harpoon>().ret = false;
        }
        else if(fired && playerControls.Player.Fire.triggered) {
            shot.GetComponent<Harpoon>().ret = true;
        }

        if(shot != null)
        {
            for (int i = 0; i < numPoints; i++)
            {
                float t = (float)i / (numPoints - 1);
                Vector3 point = Vector3.Lerp(firePoint.position, shot.GetComponent<Harpoon>().reelPoint.position, t);
                lineRenderer.SetPosition(i, point);
            }
        }
    }
}
