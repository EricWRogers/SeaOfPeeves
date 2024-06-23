using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAICombatState : MonoBehaviour
{
    public int simultaneousAttackers;
    public List<GameObject> attackers; 
    // Start is called before the first frame update
    void Start()
    {
        attackers = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
