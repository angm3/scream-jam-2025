
using Unity.VisualScripting;
using UnityEngine;



public abstract class Monster<T> : MonoBehaviour where T : Monster<T>
{
    protected StateMachine<T> stateMachine;
    protected int playerDamage;
    protected int maxHealth;
    protected int currentHealth;
    

    protected virtual void Awake()
    {
        stateMachine = new StateMachine<T>();
    }

    protected virtual void Update()
    {
        stateMachine.Update();
    }
    
    protected virtual void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }
}


public class MonsterIdleState<T> : State<T> where T : Monster<T>
{
    public MonsterIdleState(T owner, StateMachine<T> sm) : base(owner, sm) { }
}

public class MonsterChasingState<T> : State<T> where T : Monster<T>
{
    public MonsterChasingState(T owner, StateMachine<T> sm) : base(owner, sm) { }
}


public class MonsterAttackingState<T> : State<T> where T : Monster<T>
{
    public MonsterAttackingState(T owner, StateMachine<T> sm) : base(owner, sm) { }
}


public class MonsterDyingState<T> : State<T> where T : Monster<T>
{
    public MonsterDyingState(T owner, StateMachine<T> sm) : base(owner, sm) { }
}