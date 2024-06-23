using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatAudio : MonoBehaviour
{
    public BoatControls boatController;
    public AudioSource boatAudio;


    public void Update()
    {
        boatAudio.volume = Mathf.Lerp(0,1,boatController.Boat.GetComponent<Rigidbody>().velocity.sqrMagnitude / boatController.maxSpeed);
    }
}
