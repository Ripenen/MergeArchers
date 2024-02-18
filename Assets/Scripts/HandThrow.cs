using UnityEngine;

public class HandThrow : Weapon
{
    [SerializeField] private Projectile _prefab;
    [SerializeField] private AnimationClip _animation;
    [SerializeField] private int _count;
    
    public override void CreateProjectiles(Transform parent, ArcherType archerType)
    {
        Projectiles = new Projectile[_count];
        
        for (int i = 0; i < _count; i++)
        {
            var arrow = Instantiate(_prefab, parent);

            arrow.SetColliderActive(false);
            arrow.OwnArcherType = archerType;

            Projectiles[i] = arrow;
        }
    }

    public override void ResetWeapon()
    {
    }

    public override void PowerChangeAnimation(float power)
    {
    }
}