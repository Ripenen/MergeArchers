using System.Collections.Generic;
using RayFire;
using UnityEngine;

public class Bomb : Projectile
{
    [SerializeField] private float _radius;
    [SerializeField] private ParticleSystem _explosion;

    private static Dictionary<RayfireRigid, bool> d = new ();

    private void Start()
    {
        var rigids = FindObjectsOfType<RayfireRigid>();

        foreach (var rigid in rigids)
        {
            d.TryAdd(rigid, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Collided)
            return;
        
        if (other.TryGetComponent<Key>(out var key))
        {
            if (OwnArcherType == ArcherType.Player)
            {
                key.Disable();
            
                KeysView.EnableKeys(KeysView.EnabledKeys + 1);
                
                Sound.Play(Sound.Sounds.KeyUp);
            }

            return;
        }

        Physics.queriesHitTriggers = true;
        
        var colliders = Physics.OverlapSphere(transform.position, _radius);
        
        foreach (var collider in colliders)
        {
            if(collider.TryGetComponent<RayfireRigid>(out var towerTemplate) && d.ContainsKey(towerTemplate) && d[towerTemplate] == false)
            {
                towerTemplate.Demolish();

                d[towerTemplate] = true;

                var b = collider.GetComponentInParent<TowerTemplate>();

                if (b)
                {
                    foreach (var cell in b.Cells)
                    {
                        if(cell.HasArcher)
                            cell.PlacedArcher.Kill();
                    }
                }
            }
            
            if(collider.TryGetComponent<Archer>(out var archer))
            {
                archer.Kill();
            }
        }
        
        _explosion.Play(true);
        
        Sound.Play(Sound.Sounds.Explosion);
        
        Destroy(gameObject, _explosion.main.duration);
        
        Collided = true;
    }
}