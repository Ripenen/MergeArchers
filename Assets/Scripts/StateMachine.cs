using Zenject;

public class StopMachineState : State
{
    public override void Enter()
    {
        StateMachine.Stop();
    }

    public override void Exit()
    {
    }
}
public class StateMachine : IStateMachine
{
    private readonly State[] _states;

    private int _activeStateIndex;
    
    public StateMachine(State[] states)
    {
        _states = states;
    }
    
    public void Run()
    {
        EnterByActiveIndex();
    }

    public void TransitToNext()
    {
        ExitFromActive();

        _activeStateIndex++;

        EnterByActiveIndex();
    }

    public void Stop()
    {
        ExitFromActive();
    }

    public void Reload()
    {
        ExitFromActive();

        _activeStateIndex = 0;
    }

    private void EnterByActiveIndex()
    {
        if (_activeStateIndex == _states.Length)
            _activeStateIndex = 0;
        
        if (_states[_activeStateIndex] is ITickable tickable)
            MonoCached.Tick += tickable.Tick;
        
        _states[_activeStateIndex].SetStateMachine(this);
        _states[_activeStateIndex].Enter();
    }

    private void ExitFromActive()
    {
        if (_states[_activeStateIndex] is ITickable tickable)
            MonoCached.Tick -= tickable.Tick;
        
        _states[_activeStateIndex].Exit();
    }
}