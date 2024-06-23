using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using TMPro;
using UnityEngine;


public class BoatControls : MonoBehaviour
{
    public GameObject Boat;
    public TMP_Text speed;
    public float acceleration = 10f;
    public float decceleration = -5f;
    public float turnRotation = 15f;
    public float turnSpeed = 15f;
    public float maxSpeed = 25f;
    public float anchorDrag = 10f;
    public float straightenTime = 1f;
    public float minHieght = 18f;
    private float baseDrag;



    private Rigidbody rb;
    //private Vector3 direction = Vector3.forward;
    private float currentAcceleration = 0f;
    private bool anchored = false;
    private bool goingLeft = false;
    private bool goingRight = false;
    private float straightenTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = Boat.GetComponent<Rigidbody>();
        baseDrag = rb.drag;
    }

    // Update is called once per frame
    void Update()
    {
        if(!anchored && goingLeft || goingRight)
        {
            straightenTimer += Time.deltaTime;
            if (straightenTimer > straightenTime)
            {
                straight();
            }
        }
        if(rb.transform.position.y <= minHieght)
        {
            rb.gameObject.transform.position = new Vector3 (rb.gameObject.transform.position.x, minHieght, rb.gameObject.transform.position.z);
            rb.velocity = new Vector3 (rb.velocity.x, 0, rb.velocity.z);
        }
        speed.text = "Speed: " + (int)(rb.velocity.magnitude*10);
    }

    private void FixedUpdate()
    {
        if (anchored)
        {
            rb.drag = anchorDrag;
        }
        else if (!anchored)
        {
            rb.drag = baseDrag;
        }

        if (goingLeft)
        {
            //direction += new Vector3(0, -turnRotation, 0).normalized * turnSpeed * Time.deltaTime;
            Boat.transform.Rotate(Vector3.up, -turnRotation * turnSpeed * Time.deltaTime);
        }
        else if (goingRight)
        {
            //Debug.Log("going right");
            //direction += new Vector3(0, turnRotation, 0).normalized * turnSpeed * Time.deltaTime;
            Boat.transform.Rotate(Vector3.up, turnRotation * turnSpeed * Time.deltaTime);
        }

        if ( !anchored && !(rb.velocity.sqrMagnitude >= maxSpeed && currentAcceleration > 0) && !(rb.velocity.sqrMagnitude <= -maxSpeed && currentAcceleration < 0))
        {
            rb.AddForce(Boat.transform.forward * currentAcceleration * Time.deltaTime);
        }
    }

    public void accelerate()
    {
        if(currentAcceleration < maxSpeed)
            currentAcceleration += acceleration;
        Debug.Log("accelerate");
    }

    public void decelerate()
    {
        if(currentAcceleration > -maxSpeed)
            currentAcceleration += decceleration;
    }

    public void left()
    {
        straightenTimer = 0f;
        goingLeft = true;
        goingRight = false;
        Debug.Log("left");
    }

    public void right()
    {
        straightenTimer = 0f;
        goingRight = true;
        goingLeft = false;
        Debug.Log("right");
    }

    public void straight()
    {
        straightenTimer = 0f;
        goingLeft = false;
        goingRight = false;
    }

    public void anchor()
    {
        anchored = !anchored;
        currentAcceleration = 0.0f;
    }
}
