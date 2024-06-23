using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    public Transform returnPoint;
    public Transform reelPoint;
    public Transform tip;

    public float speed;
    public float rewindSpeed;
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
            rb.angularDrag = 50;
            Vector3 temp = returnPoint.position - reelPoint.position;
            rb.velocity = temp.normalized * rewindSpeed;
            transform.LookAt(returnPoint);
            if(Vector3.Distance(returnPoint.position, reelPoint.position) < .1f)
            {
                returnPoint.parent.GetComponent<HarpoonGun>().fired = false;
                returnPoint.parent.GetComponent<HarpoonGun>().lineRenderer.enabled = false;
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Hit");
        //hitSomething = true;
        //rb.velocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
        //collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //collision.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        //foreach (Collider c in GetComponents<Collider>())
        //{
        //    c.enabled = false;
        //}
        //if (collision.gameObject.tag == "Grabbable")
        //{
        //    rb.velocity = Vector3.zero;
        //    rb.angularVelocity = Vector3.zero;

        //    Destroy(collision.gameObject.GetComponent<Rigidbody>());
        //    collision.gameObject.transform.parent = tip;
        //    hitSomething = true;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");
        hitSomething = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        foreach (Collider c in GetComponents<Collider>())
        {
            c.enabled = false;
        }
        if (other.gameObject.tag == "Grabbable")
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Destroy(other.gameObject.GetComponent<Rigidbody>());
            other.gameObject.transform.parent = tip;
            hitSomething = true;
        }
    }
}
