using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemyArchersAimState : State, IDataProvider<Trajectory>
{
    private readonly Tower _tower;
    private readonly float _aimTime;
    private readonly InGameView _view;

    private float _timeFromStart;

    private Vector3 _targetDirection;
    private float _targetPower;

    public EnemyArchersAimState(Tower tower, float aimTime, InGameView view)
    {
        _tower = tower;
        _aimTime = aimTime;
        _view = view;
    }

    public Trajectory Data => new (_targetDirection, _targetPower, 17.5f);

    public override void Enter()
    {
        if(_tower.TowerEffect is not null)
            _tower.TowerEffect.OnStartAim(_tower.AliveArchers);
        
        var x = Random.Range(-1f, -0.25f);

        _targetDirection = new Vector3(x, 1 + x);
        _targetPower = Random.Range(0.55f, 1.1f);
        
        foreach (var archer in _tower.AliveArchers)
        {
            archer.SetColliders(false);
            archer.StartAim();
        }

        MonoCached.LateTick += LateTick;
    }

    private void LateTick()
    {
        var value = _timeFromStart / _aimTime;
        var power = value * _targetPower;
        var direction = Vector3.Lerp(Vector3.left, _targetDirection, value);
        
        foreach (var enemyArcher in _tower.AliveArchers)
        {
            if(!enemyArcher)
                continue;
            
            enemyArcher.AimPower(power);
            enemyArcher.AimAngle(direction);
        }
        
        _timeFromStart += Time.deltaTime;
        
        if(value >= 1)
        {
            StateMachine.TransitToNext();
        }
    }

    public override void Exit()
    {
        Sound.Play(_tower.AliveArchers.First().ActiveWeapon is HandThrow
            ? Sound.Sounds.HandThrowShoot
            : Sound.Sounds.Shoot);

        MonoCached.LateTick -= LateTick;
        _timeFromStart = 0;

        _view.UpBombLoad();
    }
}