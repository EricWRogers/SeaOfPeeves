
using UnityEngine;
public class ShipTeleporterTrigger : MonoBehaviour
{
    private string playerTag = "Player"; 
    private Collider _collider;
    private bool _isColliding;
    public Transform shipDeck;
    public void Start()
    {
        _collider = GetComponent<Collider>();
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag(playerTag))
        {
            _isColliding = true;
            Vector3 shipDeckOffset = new Vector3(shipDeck.transform.position.x, shipDeck.transform.position.y + 1,
                shipDeck.transform.position.z);
            collider.transform.position = shipDeckOffset;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        _isColliding = false;
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
        Gizmos.DrawLine(transform.position + transform.rotation * (top + Vector3.left * radius),
            transform.position + transform.rotation * (bottom + Vector3.left * radius));
        Gizmos.DrawLine(transform.position + transform.rotation * (top + Vector3.right * radius),
            transform.position + transform.rotation * (bottom + Vector3.right * radius));
        Gizmos.DrawLine(transform.position + transform.rotation * (top + Vector3.forward * radius),
            transform.position + transform.rotation * (bottom + Vector3.forward * radius));
        Gizmos.DrawLine(transform.position + transform.rotation * (top + Vector3.back * radius),
            transform.position + transform.rotation * (bottom + Vector3.back * radius));
    }
}