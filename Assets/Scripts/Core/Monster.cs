using UnityEngine;

public abstract class Monster : MonoBehaviour
{

    public StateMachine<Monster> stateMachine;

    public Monster()
    {
        stateMachine = new StateMachine<Monster>();
        // set initial state
        stateMachine.ChangeState(new MonsterIdleState(this, stateMachine));
    }

    public abstract void Chase();

    private void Update()
    {
        stateMachine.Update();
    }
    
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }
}

public class MonsterIdleState : State<Monster>
{
    public MonsterIdleState(Monster owner, StateMachine<Monster> sm) : base(owner, sm) { }

    public override void Enter()
    {
        // Debug.Log("Entered idle state");
    }

    public override void Update()
    {
        if ((GameManager.Instance.GetPlayer().transform.position - this.owner.gameObject.transform.position).magnitude < 10f)
        {
            stateMachine.ChangeState(new MonsterChasingState(owner, stateMachine));
        }
    }
}


public class MonsterChasingState : State<Monster>
{
    public MonsterChasingState(Monster owner, StateMachine<Monster> sm) : base(owner, sm) { }

    public override void Enter()
    {
        // Debug.Log("Entered chasing state");
    }

    public override void Update()
    {
        this.owner.Chase();

        if ((GameManager.Instance.GetPlayer().transform.position - this.owner.gameObject.transform.position).magnitude > 10f)
        {
            stateMachine.ChangeState(new MonsterIdleState(owner, stateMachine));
        }
    }
}

