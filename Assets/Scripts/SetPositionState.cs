using DG.Tweening;
using UnityEngine;

public class SetPositionState : State
{
    private readonly Transform _positionApplier;
    private readonly Transform _target;

    public SetPositionState(Transform positionApplier, Transform target)
    {
        _positionApplier = positionApplier;
        _target = target;
    }
    
    public override void Enter()
    {
        _positionApplier.DOKill();
        _positionApplier.position = _target.position;
        _positionApplier.rotation = _target.rotation;
        
        StateMachine.TransitToNext();
    }

    public override void Exit()
    {
    }
}