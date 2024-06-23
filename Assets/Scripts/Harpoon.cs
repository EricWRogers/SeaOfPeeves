using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    public Transform returnPoint;
    public Transform reelPoint;
    public Transform tip;

    public int damage = 50;

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
            rb.velocity = -transform.forward * speed;
        }
        else
        {
            rb.angularDrag = 50;
            Vector3 move = (returnPoint.position - reelPoint.position);

            if ((move).magnitude > (move.normalized).magnitude)
                rb.velocity = move * rewindSpeed;
            else
                rb.velocity = move.normalized * rewindSpeed;

            transform.LookAt(returnPoint);
            if(Vector3.Distance(returnPoint.position, reelPoint.position) < Mathf.Max((move * rewindSpeed * Time.deltaTime).magnitude, (move * rewindSpeed * Time.deltaTime).normalized.magnitude))
            {
                returnPoint.parent.GetComponent<HarpoonGun>().fired = false;
                returnPoint.parent.GetComponent<HarpoonGun>().lineRenderer.enabled = false;
                Destroy(gameObject);
            }
        }
    }

    public void TimeOut()
    {
        ret = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Grabbable" || other.isTrigger == false)
        {
            Debug.Log("Hit");
            hitSomething = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            
            foreach (Collider c in GetComponents<Collider>())
            {
                c.enabled = false;
            }
        
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (other.gameObject.GetComponent<SuperPupSystems.Helper.Health>() == null)
            {
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                other.gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                Destroy(other.gameObject.GetComponent<Rigidbody>());
                other.gameObject.transform.parent = tip;
            }
            hitSomething = true;
        }

        other.gameObject.GetComponent<SuperPupSystems.Helper.Health>()?.Damage(damage);
    }
}
