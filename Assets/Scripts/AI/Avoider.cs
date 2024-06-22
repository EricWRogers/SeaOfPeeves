using DefaultNamespace;
using UnityEngine;
using UnityEngine.Serialization;

public class Avoider : MonoBehaviour
{
    public string packTag = "Agent";
    public Transform avoidEnemy = null; 
    public MonsterAgent monsterAgent;
    
    private Collider _collider;
    private bool _isColliding;
    public void Start()
    {
        _collider = GetComponent<Collider>();
        monsterAgent = transform.parent.gameObject.GetComponent<MonsterAgent>();
    }
    public void OnTriggerEnter(Collider collider)
    {
        if(monsterAgent == null )
        {
            return;
        }

        if (collider.gameObject.CompareTag(packTag) && collider.transform != transform.parent && avoidEnemy == null)
        {
            _isColliding = true;
            if (collider.gameObject != monsterAgent.playerGameObject)
            {
                avoidEnemy = collider.transform;
            }
        }
    }

    public void OnTriggerStay(Collider collider)
    {
        if (monsterAgent == null)
        {
            return;
        }

        if (collider.gameObject.CompareTag(packTag) && collider.transform != transform.parent )
        {
            _isColliding = true;
            if (avoidEnemy == null)
            {
                if (collider.gameObject != monsterAgent.playerGameObject)
                {
                    avoidEnemy = collider.transform;
                }
            }
            
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (monsterAgent == null )
        {
            return;
        }
        if (collider.gameObject.CompareTag(packTag) && collider.transform != transform.parent && avoidEnemy)
        {

            if ( collider.gameObject == avoidEnemy.gameObject)
            {
                _isColliding = false;
                avoidEnemy = null;
            }

        }
        
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
        else if (_collider is SphereCollider)
        {
            SphereCollider sphereCollider = (SphereCollider)_collider;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            Gizmos.DrawWireSphere(sphereCollider.center, sphereCollider.radius);
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