using System;
using UnityEngine;

internal class Horse : MonoBehaviour, IDamageable
{
    public ArcherPlaceCell _cell;
    public Animator _animator;
    public Rigidbody[] _ragDoll;
    public Collider[] _colliders;
    public Collider _main;

    public Archer PlacedArcher => _cell.PlacedArcher;

    private void Awake()
    {
        foreach (var collider in _colliders)
        {
            collider.enabled = false;
        }
    }

    public void TakeDamage()
    {
        if(PlacedArcher)
            PlacedArcher.Kill();

        _animator.enabled = false;
        _main.enabled = false;
        
        foreach (var rigidbody in _ragDoll)
        {
            rigidbody.isKinematic = false;
        }

        foreach (var collider in _colliders)
        {
            collider.enabled = true;
        }
    }
}