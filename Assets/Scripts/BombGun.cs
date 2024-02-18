using UnityEngine;

public class BombGun : Weapon
{
    [SerializeField] private Bomb _bomb;
    
    public override void CreateProjectiles(Transform parent, ArcherType archerType)
    {
        var bomb = Instantiate(_bomb, parent);
        
        bomb.Translate(parent.forward * 0.25f);
        
        bomb.SetColliderActive(false);
        
        Projectiles = new Projectile[] { bomb };
    }

    public override void ResetWeapon()
    {
    }

    public override void PowerChangeAnimation(float power)
    {
    }
}