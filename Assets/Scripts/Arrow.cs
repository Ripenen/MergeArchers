using System;
using DG.Tweening;
using UnityEngine;

public class Arrow : Projectile
{
    public void OnTriggerEnter(Collider body)
    {
        if (Collided)
            return;
        
        if (body.TryGetComponent<Key>(out var key))
        {
            if (OwnArcherType == ArcherType.Player)
            {
                key.Disable();
            
                KeysView.EnableKeys(KeysView.EnabledKeys + 1);
                
                Sound.Play(Sound.Sounds.KeyUp);
            }

            return;
        }

        if (body.TryGetComponent<RagDollCollider>(out var archer) && archer.Owner.ArcherType != OwnArcherType)
        {
            archer.Owner.TakeDamage();
        }
        
        if (body.TryGetComponent<Archer>(out var d) && d.ArcherType != OwnArcherType)
        {
            d.TakeDamage();

            return;
        }
        
        if (body.TryGetComponent<Balloon>(out var ballon) && OwnArcherType == ArcherType.Player)
        {
            ballon.TakeDamage();
        }
        
        if (body.TryGetComponent<Horse>(out var horse) && OwnArcherType == ArcherType.Player)
        {
            horse.TakeDamage();

            transform.parent = horse.transform;
        }

        if (body.attachedRigidbody)
        {
            transform.parent = body.transform;
        }
        
        SetColliderActive(false);
        
        Sound.Play(Sound.Sounds.Hit);

        Camera.DOKill();
        Camera.DOShakePosition(0.1f, 0.25f, 1, 180, false, ShakeRandomnessMode.Harmonic);

        Collided = true;
    }
}