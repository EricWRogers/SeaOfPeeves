
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using DefaultNamespace;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.Splines;
    using Random = UnityEngine.Random;

    public class ShipMonsterSpawner : MonoBehaviour
    {
        public GameObject monsterPrefab;
        public List<GameObject> spawnedMonsters;

        public float spawnAmount;
        private string playerTag = "Player";
        public float spawnInterval = 2f;
        public float spawnRadius = 3f;

        public GameObject playerGameObject;
        public Transform SpawnEndPoint;
        private bool spawning;
        public SplineContainer spawnSpline;
        // Start is called before the first frame update
        public void Start()
        {
            
            spawnedMonsters = new List<GameObject>();
            playerGameObject = GameObject.FindGameObjectWithTag(playerTag);
        }

        private void Update()
        {
            if (!spawning)
            {
                StartCoroutine(SpawnMonsters());

            }
        }

        private IEnumerator SpawnMonsters()
        {
            spawning = true;
            while (spawnedMonsters.Count < spawnAmount)
            {
                GameObject monster = Instantiate(monsterPrefab, transform.position, quaternion.identity);
                monster.GetComponent<MonsterAgent>().playerGameObject = playerGameObject;
                monster.GetComponent<MonsterAgent>().playerAICombatState = playerGameObject.GetComponent<PlayerAICombatState>();
                monster.GetComponent<MonsterAgent>().splineAnimation.Container = spawnSpline;
                monster.GetComponent<MonsterAgent>().splineAnimation.Play(); 
                monster.transform.LookAt(playerGameObject.transform);
                spawnedMonsters.Add(monster);
                yield return new WaitForSeconds(spawnInterval);
            }
        }

       

        private Vector3 GetRandomPositionOnShip()
        {
            Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
            randomDirection.y = 0; // Ensure the position is on the same horizontal plane
            return SpawnEndPoint.position + randomDirection;
        }
        
    }
