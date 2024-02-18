using System.Linq;
using UnityEngine;

public class Poison : Magic
{
    [SerializeField] private ParticleSystem _poison;
    
    public override void Run(PlayerTower tower, LevelTower d, Camera camera, GameFactory factory,
        GameBehaviour behaviour)
    {
        var target = d;
        var transformPosition = target.Center;
        _poison.transform.position = transformPosition + new Vector3(0, 5, 1);
        
        target.MakeParentOf(this);
        
        _poison.Play();

        var effect = new PoisonTowerEffect();

        target.TowerEffect = effect;
        
        foreach (var archer in target.AliveArchers)
        {
            effect.Setup(archer);
            archer.TakeDamage();
        }

        if (!target.AliveArchers.Any())
            StartCoroutine(behaviour.Win());
        
        camera.transform.position = transformPosition + new Vector3(-8f, 5f, -8f);
        camera.transform.LookAt(transformPosition);
        
        Sound.Play(Sound.Sounds.Poison);
        
        MonoCached.Timer(this, _poison.main.duration, () =>
        {
            camera.transform.SetPositionAndRotation(tower.AimCameraPosition.position, tower.AimCameraPosition.rotation);
        });
    }
}