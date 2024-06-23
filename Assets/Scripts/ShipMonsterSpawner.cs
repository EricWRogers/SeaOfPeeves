
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
        public float RandomShipSpawnMin;
        public float RandomShipSpawnMax;
        public float RandomShipSpawn;
        public float lastSpawnTime = 0;
        public float spawnInterval = 2f;
        public float spawnRadius = 3f;
        public float maxSpawnAmount;
        public GameObject playerGameObject;
        private bool spawning;
        public SplineContainer spawnSpline;
        // Start is called before the first frame update
        public void Start()
        {
            
            spawnedMonsters = new List<GameObject>();
            playerGameObject = GameObject.FindGameObjectWithTag(playerTag);
            RandomShipSpawn = Random.Range(RandomShipSpawnMin, RandomShipSpawnMax);
            
        }

        private void Update()
        {
            if ((Time.fixedTime - lastSpawnTime) > RandomShipSpawn)
            {
                if (!spawning)
                {
                    spawning = true;
                    StartCoroutine(SpawnMonsters());
                }
            }

            for (int i = 0; i < spawnedMonsters.Count; i++)
            {
                if (spawnedMonsters[i].gameObject == null)
                {
                    spawnedMonsters.Remove(spawnedMonsters[i]);
                }
            }
            
        }

        private IEnumerator SpawnMonsters()
        {
            int counter = 0;
            while (counter < spawnAmount && spawnedMonsters.Count < maxSpawnAmount)
            {
                GameObject monster = Instantiate(monsterPrefab, transform.position, transform.rotation);
                monster.GetComponent<MonsterAgent>().playerGameObject = playerGameObject;
                monster.GetComponent<MonsterAgent>().playerAICombatState = playerGameObject.GetComponent<PlayerAICombatState>();
                monster.GetComponent<MonsterAgent>().splineAnimation.Container = spawnSpline;
                monster.GetComponent<MonsterAgent>().splineAnimation.Play(); 
                monster.GetComponent<MonsterAgent>().monsterAnimator.SetTrigger("Climb");
                monster.GetComponent<MonsterAgent>().monsterAnimator.SetBool("Climbing", true);
                monster.GetComponent<MonsterAgent>().combatStates.isClimbing = true;
                monster.transform.LookAt(playerGameObject.transform);
                spawnedMonsters.Add(monster);
                counter++;
                yield return new WaitForSeconds(spawnInterval);
            }

            lastSpawnTime = Time.fixedTime;
            spawning = false;
            RandomShipSpawn = Random.Range(RandomShipSpawnMin, RandomShipSpawnMax);
            
        }
        
    }
