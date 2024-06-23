using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public List<GameObject> spawnedMonsters;
    
    public float spawnAmount; 
    private string playerTag = "Player";    
    private Collider _collider;
    private bool _isColliding;
    public float spawnInterval = 2f;
    public float despawnDelay = 5f;
    public float spawnRadius = 3f;
    private GameObject playerGameObject;
    // Start is called before the first frame update

   
    public void Start()
    {
        _collider = GetComponent<Collider>();
        spawnedMonsters = new List<GameObject>();
    }

    public void OnTriggerEnter(Collider collider)
    {
        StopCoroutine(DespawnMonsters());
        if (collider.gameObject.CompareTag(playerTag))
        {
            playerGameObject = collider.gameObject;
            _isColliding = true;
            StartCoroutine(SpawnMonsters());
        }
    }
     
    public void OnTriggerExit(Collider collider)
    {
        //Todo: Start a coroutine that despawns the creatures after a set amount of time 
        _isColliding = false;
        StartCoroutine(DespawnMonsters());
    }
    
    private IEnumerator SpawnMonsters()
    {
        while (_isColliding && spawnedMonsters.Count < spawnAmount)
        {
            GameObject monster = Instantiate(monsterPrefab, GetRandomPosition(), transform.rotation);
            monster.GetComponent<MonsterAgent>().playerGameObject = playerGameObject;
            monster.transform.LookAt(playerGameObject.transform);
            spawnedMonsters.Add(monster);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private IEnumerator DespawnMonsters()
    {
        yield return new WaitForSeconds(despawnDelay);

        if (!_isColliding)
        {
            foreach (GameObject monster in spawnedMonsters)
            {
                if (monster != null)
                {
                    Destroy(monster);
                }
            }
            spawnedMonsters.Clear();
        }
    }
    private Vector3 GetRandomPosition()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection.y = 0; // Ensure the position is on the same horizontal plane
        return transform.position + randomDirection;
    }
    
    void OnDrawGizmos()
    {
        if (_collider == null)
            return;

        Gizmos.color = _isColliding ? Color.red : Color.green;

        if (_collider is BoxCollider)
        {
            BoxCollider boxCollider = (BoxCollider)_collider;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }
        else if (_collider is CapsuleCollider)
        {
            CapsuleCollider capsuleCollider = (CapsuleCollider)_collider;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            DrawWireCapsule(capsuleCollider);
        }
  
    }

    void DrawWireCapsule(CapsuleCollider capsuleCollider)
    {
        float radius = capsuleCollider.radius;
        float height = capsuleCollider.height / 2 - radius;

        Vector3 center = capsuleCollider.center;
        Vector3 top = center + Vector3.up * height;
        Vector3 bottom = center + Vector3.down * height;

        Gizmos.DrawWireSphere(transform.position + transform.rotation * top, radius);
        Gizmos.DrawWireSphere(transform.position + transform.rotation * bottom, radius);
        Gizmos.DrawLine(transform.position + transform.rotation * (top + Vector3.left * radius), transform.position + transform.rotation * (bottom + Vector3.left * radius));
        Gizmos.DrawLine(transform.position + transform.rotation * (top + Vector3.right * radius), transform.position + transform.rotation * (bottom + Vector3.right * radius));
        Gizmos.DrawLine(transform.position + transform.rotation * (top + Vector3.forward * radius), transform.position + transform.rotation * (bottom + Vector3.forward * radius));
        Gizmos.DrawLine(transform.position + transform.rotation * (top + Vector3.back * radius), transform.position + transform.rotation * (bottom + Vector3.back * radius));
    } 
    
}
