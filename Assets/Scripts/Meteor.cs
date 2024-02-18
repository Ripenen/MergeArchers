using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Meteor : Magic
{
    [SerializeField] private ParticleSystem _firepath;
    [SerializeField] private ParticleSystem _explosion;
    
    private Tower _target;
    private Vector3 _center;
    private Camera _camera;
    private PlayerTower _tower;

    private bool _active;

    public override void Run(PlayerTower tower, LevelTower target, Camera camera, GameFactory factory,
        GameBehaviour behaviour)
    {
        _tower = tower;
        _camera = camera;
        _target = target;

        var ra = Random.insideUnitCircle * 5;
        
        _center = target.Center + new Vector3(ra.x, 0, ra.y);
        
        transform.position = _center + new Vector3(0, 20, 8);

        Sound.Play(Sound.Sounds.Meteor);

        StartCoroutine(DestroyIfNotCollide());

        _center = (_center - transform.position).normalized;
    }

    private IEnumerator DestroyIfNotCollide()
    {
        yield return new WaitForSeconds(0.75f);
        
        if(_active)
            yield break;

        yield return OnTriggerEnter(null);
    }

    private void Update()
    {
        if(_active)
            return;
        
        transform.Translate(_center * (Time.deltaTime * 30));
    }
    
    private IEnumerator OnTriggerEnter(Collider collider)
    {
        if(_active)
            yield break;

        _active = true;
        
        _explosion.Play();
        
        foreach (var archer in _target.AliveArchers.Skip(1))
        {
            var dist = Vector3.Distance(transform.position, archer.transform.position);
            
            if(dist <= 4)
                archer.Kill();
        }

        _camera.DOShakePosition(0.3f, 1, 3, 180, false, ShakeRandomnessMode.Harmonic);

        yield return new WaitForSeconds(_explosion.main.duration + 0.5f);
        
        _camera.transform.SetPositionAndRotation(_tower.AimCameraPosition.position, _tower.AimCameraPosition.rotation);
        
        Destroy(gameObject);

        _camera.DOKill();
    }
}