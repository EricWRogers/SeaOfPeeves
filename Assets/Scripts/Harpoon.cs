using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    public Transform returnPoint;
    public Transform reelPoint;
    public Transform tip;

    public float speed;
    public bool ret = false;
    private bool hitSomething = false;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        hitSomething = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!ret && !hitSomething)
        {
            rb.AddForce(-transform.forward * speed);
        }
        else
        {
            Vector3 temp = returnPoint.position - reelPoint.position;
            rb.AddForce(temp.normalized * speed);
            transform.LookAt(returnPoint);
            if(Vector3.Distance(returnPoint.position, reelPoint.position) < .1f)
            {
                returnPoint.parent.GetComponent<HarpoonGun>().fired = false;
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        hitSomething = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        foreach (Collider c in GetComponents<Collider>())
        {
            c.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Grabbable")
        {
            other.transform.parent = tip;
            hitSomething = true;
        }
    }
}
