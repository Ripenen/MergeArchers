using UnityEngine;

public class RagDollCollider : MonoBehaviour
{
    private Archer _owner;

    public Archer Owner => _owner;
    public Rigidbody Rigidbody;

    public void Init(Archer owner)
    {
        _owner = owner;

        Rigidbody = GetComponent<Rigidbody>();
    }
}