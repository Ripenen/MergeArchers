using UnityEngine;

public class Heal : Magic
{
    [SerializeField] private ParticleSystem _heal;
    
    public override void Run(PlayerTower tower, LevelTower target, Camera camera, GameFactory factory,
        GameBehaviour behaviour)
    {
        _heal.transform.position = tower.Center;
        
        _heal.Play();
        
        tower.Reload(factory);
        
        Destroy(gameObject, _heal.main.duration);
        
        Sound.Play(Sound.Sounds.Heal);
    }
}