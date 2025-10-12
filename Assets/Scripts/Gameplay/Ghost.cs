using UnityEngine;



public class Ghost : Monster<Ghost>
{
    private void Start()
    {
        stateMachine.ChangeState(new GhostIdleState(this, stateMachine));
    }

    public bool checkIfVelocityIsForward(Rigidbody rb)
    {
        return Vector3.Dot(rb.transform.forward, rb.linearVelocity) > 0;
    }
}


public class GhostIdleState : MonsterIdleState<Ghost>
{
    public GhostIdleState(Ghost owner, StateMachine<Ghost> sm) : base(owner, sm) { }

    public override void Enter()
    {
        Debug.Log("Entered Ghost Idle");
    }

    public override void Update() 
    {
        if ((GameManager.Instance.GetPlayer().transform.position - this.owner.gameObject.transform.position).magnitude < 10f)
        {
            stateMachine.ChangeState(new GhostChasingState(owner, stateMachine));
        }
    }
}


public class GhostChasingState : MonsterChasingState<Ghost>
{
    public GhostChasingState(Ghost owner, StateMachine<Ghost> sm) : base(owner, sm) { }

    public override void Enter()
    {

        Debug.Log("Ghost is chasing");
    }

    public override void Update()
    {

        if (owner.checkIfVelocityIsForward(GameManager.Instance.GetPlayer().GetComponent<Rigidbody>()))
        {
            owner.gameObject.transform.position = Vector3.MoveTowards(
                owner.gameObject.transform.position,
                GameManager.Instance.GetPlayer().transform.position + GameManager.Instance.GetPlayer().GetComponent<Rigidbody>().linearVelocity * 2f,
                0.01f);
        }
        else
        {
            owner.gameObject.transform.position = Vector3.MoveTowards(
                owner.gameObject.transform.position,
                GameManager.Instance.GetPlayer().transform.position,
                0.01f);
        }

        if ((GameManager.Instance.GetPlayer().transform.position - owner.transform.position).magnitude > 15f)
        {
            stateMachine.ChangeState(new GhostIdleState(owner, stateMachine));
        }
    }
}
