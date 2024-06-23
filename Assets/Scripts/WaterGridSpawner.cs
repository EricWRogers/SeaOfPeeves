using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGridSpawner : MonoBehaviour
{
    public GameObject waterSurfacePrefab;

    public int gridSize = 100;

    public int yLevel = 0;

    void Start()
    {
        float offset = (gridSize - 1) / 2.0f * gridSize;

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 spawnPosition = new Vector3(x * 100 - offset, yLevel, z * 100 - offset);
                GameObject gobj = Instantiate(waterSurfacePrefab, spawnPosition, Quaternion.identity);
                gobj.transform.parent = transform;
            }
        }
    }

    void Update()
    {
        
    }
}
