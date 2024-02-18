using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public Projectile[] Projectiles { get; protected set; }

    public abstract void CreateProjectiles(Transform parent, ArcherType archerType);

    public abstract void ResetWeapon();

    public void SetForward(Vector3 forward)
    {
        foreach (var arrow in Projectiles)
        {
            arrow.SetForward(forward);
        }
        
        if(this is BombGun)
            transform.forward = forward;
    }
    
    public abstract void PowerChangeAnimation(float power);
}