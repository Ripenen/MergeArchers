public abstract class State
{
    public virtual bool Skip => false;
    
    protected IStateMachine StateMachine { get; private set; }
    
    public abstract void Enter();
    public abstract void Exit();

    public void SetStateMachine(IStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }
}