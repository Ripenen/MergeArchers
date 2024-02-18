using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ArcherSkeleton : MonoBehaviour
{
    [SerializeField] private Transform _palmForBow;
    [SerializeField] private Transform _body;
    [SerializeField] private Transform _bowHand;
    [SerializeField] private Renderer _renderer;
    [SerializeField] private List<Rigidbody> _ragDoll;
    [SerializeField] private List<Collider> _ragDollColliders;
    [SerializeField] private List<RagDollCollider> _ragDollCollidersComp;
    
    public void Init(Archer archer)
    {
        foreach (var ragDollCollider in _ragDollCollidersComp)
        {
            ragDollCollider.Init(archer);
        }
    }

    public void SetBodyForward(Vector3 forward)
    {
        if(forward.sqrMagnitude == 0)
            return;

        _body.rotation = Quaternion.LookRotation(forward, _body.up);
    }

    public void SetRagDoll(bool value)
    {
        _ragDoll.ForEach(x => x.isKinematic = !value);
    }

    public void MoveBowHand(float power)
    {
        if(!_bowHand)
            return;
        
        _bowHand.Translate(Vector3.right * Mathf.Lerp(-0.75f, -0.25f, power));
    }

    public void PrepareToAim(Weapon bow, ArcherType type)
    {
        bow.CreateProjectiles(bow is Bow ? _palmForBow : bow.transform, type);
    }

    public void SetColliders(bool value)
    {
        _ragDollColliders.ForEach(x => x.enabled = value);
    }

    public void SetColor(Color color)
    {
        _renderer.material.DOColor(color, 1);
    }

    public void AddForce(Vector3 direction, float force)
    {
        foreach (var rigidbody in _ragDoll)
        {
            rigidbody.AddForce(direction * force, ForceMode.Impulse);
        }
    }
}