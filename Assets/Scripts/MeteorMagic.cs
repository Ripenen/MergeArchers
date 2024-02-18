using System.Collections;
using UnityEngine;

public class MeteorMagic : Magic
{
    [SerializeField] private Meteor[] _meteors;
    
    public override void Run(PlayerTower tower, LevelTower target, Camera camera, GameFactory factory,
        GameBehaviour behaviour)
    {
        _meteors[0].transform.position = new Vector3(0, 100, 0);
        _meteors[1].transform.position = new Vector3(0, 76, 0);
        _meteors[2].transform.position = new Vector3(0, 52, 0);
        
        camera.transform.position = target.Center + new Vector3(-8f, 5f, -8f);
        camera.transform.LookAt(target.Center);
        
        StartCoroutine(RunS(tower, target, camera, factory, behaviour));
    }

    private IEnumerator RunS(PlayerTower tower, LevelTower target, Camera camera, GameFactory factory, GameBehaviour behaviour)
    {
        foreach (var meteor in _meteors)
        {
            meteor.Run(tower, target, camera, factory, behaviour);

            yield return new WaitForSeconds(0.5f);
        }
    }
}