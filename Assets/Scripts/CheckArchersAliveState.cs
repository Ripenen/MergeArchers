using System;
using System.Linq;

public class CheckArchersAliveState : State
{
    private readonly Tower _tower;
    private readonly Action _onAllKilled;
    
    public CheckArchersAliveState(Tower tower, Action onAllKilled)
    {
        _tower = tower;
        _onAllKilled = onAllKilled;
    }

    public override void Enter()
    {
        if (!_tower.AliveArchers.Any())
        {
            StateMachine.Stop();
            _onAllKilled?.Invoke();
        }
        else
        {
            StateMachine.TransitToNext();
        }
    }

    public override void Exit()
    {
    }
}