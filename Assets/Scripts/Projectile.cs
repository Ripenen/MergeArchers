using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Collider _collider;
    [SerializeField] private TrailRenderer _trail;
    
    public bool Collided { get; protected set; }
    public Vector3 Position => transform.position;
    
    [HideInInspector] public KeysView KeysView;

    [HideInInspector] public ArcherType OwnArcherType;
    
    private Vector3 _offset;
    public float ZOffset;
    
    [HideInInspector] public Camera Camera;

    public void SetForward(Vector3 forward)
    {
        transform.forward = forward + _offset;
    }

    public void SetColliderActive(bool value)
    {
        _collider.enabled = value;
        _trail.enabled = value;
    }

    public void Translate(Vector3 direction)
    {
        var transform1 = transform;

        direction.z = ZOffset;
        
        transform1.position += direction;
        
        if(direction.magnitude > 0 && this is not Axe)
            transform1.forward = direction;

        switch (transform1.position.y)
        {
            case <= -20:
            case >= 30:
                Collided = true;
                break;
        }
    }

    public void SetForwardOffset(Vector3 offset)
    {
        _offset = offset;
    }
}