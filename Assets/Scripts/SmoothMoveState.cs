using DG.Tweening;
using UnityEngine;

public class MoveToPlayerAimState : State
{
    private readonly PlayerTower _tower;
    private readonly Transform _applier;
    private readonly float _duration;

    public MoveToPlayerAimState(PlayerTower tower, Transform applier, float duration)
    {
        _tower = tower;
        _applier = applier;
        _duration = duration;
    }
    
    public override void Enter()
    {
        _applier.DOKill();

        if (_duration == 0)
        {
            _applier.transform.position = _tower.AimCameraPosition.position;
            _applier.rotation = _tower.AimCameraPosition.rotation;
            StateMachine.TransitToNext();
        }
        else
        {
            _applier.DOMove(_tower.AimCameraPosition.position, _duration * 0.75f).OnComplete(StateMachine.TransitToNext);
            _applier.DORotateQuaternion(_tower.AimCameraPosition.rotation, _duration * 0.75f);
        }
    }

    public override void Exit()
    {
    }
}

public class MoveToPlayerMergeState : State
{
    private readonly PlayerTower _tower;
    private readonly Transform _applier;

    public MoveToPlayerMergeState(PlayerTower tower, Transform applier)
    {
        _tower = tower;
        _applier = applier;
    }
    
    public override void Enter()
    {
        _applier.DOKill();

        _applier.position = _tower.MergeCameraPosition.position;
        _applier.rotation = _tower.MergeCameraPosition.rotation;

        StateMachine.TransitToNext();
    }

    public override void Exit()
    {
    }
}