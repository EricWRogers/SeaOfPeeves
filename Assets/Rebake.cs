using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor.SceneManagement;
using UnityEngine;

public class Rebake : MonoBehaviour
{
    public NavMeshSurface boatMesh;

    public bool reBaked;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (reBaked)
        {
            boatMesh.BuildNavMesh();
            reBaked = true;
        }
        
    }
}
