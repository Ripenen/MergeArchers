using UnityEngine;


public class Bow : Weapon
{
    [SerializeField] private Arrow _arrowPrefab;
    [SerializeField] private AnimationClip _animation;
    [SerializeField] private int _arrowsCount;
    
    public override void CreateProjectiles(Transform parent, ArcherType archerType)
    {
        Projectiles = new Projectile[_arrowsCount];
        var angleRange = 5;
        
        for (int i = 0; i < _arrowsCount; i++)
        {
            var arrow = Instantiate(_arrowPrefab, parent);

            arrow.SetColliderActive(false);
            arrow.OwnArcherType = archerType;
            
            var offset = new Vector3(0, Mathf.Sin(angleRange * Mathf.Deg2Rad));
                
            arrow.SetForwardOffset(offset);

            if (i % 2 == 0)
                angleRange += 5;
            
            angleRange *= -1;
            
            Projectiles[i] = arrow;
        }
    }

    public override void ResetWeapon()
    {
        _animation.SampleAnimation(gameObject, 0);
    }

    public override void PowerChangeAnimation(float power)
    {
        if(!_animation)
            return;
        
        var time = _animation.length * power;
        
        _animation.SampleAnimation(gameObject, time);
    }
}