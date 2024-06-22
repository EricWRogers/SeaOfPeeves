using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class BoatControls : MonoBehaviour
{
    public GameObject Boat;
    public TMP_Text speed;
    public float acceleration = 10f;
    public float decceleration = -5f;
    public float turnSpeed = 15f;
    public float maxSpeed = 25f;
    public float anchorDrag = 10f;
    private float baseDrag;

    private Rigidbody rb;
    private Vector3 direction = Vector3.forward;
    private float currentAcceleration = 0f;
    private bool anchored = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = Boat.GetComponent<Rigidbody>();
        baseDrag = rb.drag;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if ( !anchored && !(rb.velocity.sqrMagnitude >= maxSpeed && currentAcceleration > 0) && !(rb.velocity.sqrMagnitude <= -maxSpeed && currentAcceleration < 0))
        {
            rb.AddForce(direction * currentAcceleration * Time.deltaTime);
        }
        else if(anchored)
        {
            rb.drag = anchorDrag;
        }
        else if (!anchored)
        {
            rb.drag = 0;
        }
    }

    public void accelerate()
    {
        currentAcceleration += acceleration;
        Debug.Log("accelerate");
    }

    public void decelerate()
    {
        currentAcceleration += decceleration;
    }

    public void left()
    {
        
    }

    public void right()
    {

    }

    public void anchor()
    {
        anchored = !anchored;
        currentAcceleration = 0.0f;
    }
}
