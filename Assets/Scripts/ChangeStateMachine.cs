public class ChangeStateMachine : State
{
    private readonly StateMachine _stateMachine;

    public ChangeStateMachine(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }
    
    public override void Enter()
    {
        StateMachine.Stop();
    }

    public override void Exit()
    {
        _stateMachine.Run();
    }
}