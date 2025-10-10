using UnityEngine;

public abstract class State<T>
{
    protected T owner;
    protected StateMachine<T> stateMachine;

    public State(T owner, StateMachine<T> stateMachine)
    {
        this.owner = owner;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}