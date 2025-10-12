using UnityEngine;
using System.Collections;



public class Ghost : Monster<Ghost>
{
    
    private void Start()
    {
        stateMachine.ChangeState(new GhostIdleState(this, stateMachine));
        playerDamage = 69;
    }

    public bool checkIfVelocityIsForward(Rigidbody rb)
    {
        return Vector3.Dot(rb.transform.forward, rb.linearVelocity) > 0;
    }
    
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collison detected in ghost");

        if (other.gameObject.CompareTag("Player"))
        {
            if (stateMachine.CurrentState is GhostAttackingState)
            {
                Debug.Log("Player hit by ghost");
                EventBus.Publish(new PlayerDamageEvent(playerDamage));
            }
            else if (stateMachine.CurrentState is GhostChasingState || stateMachine.CurrentState is GhostIdleState)
            {
                Debug.Log("Ghost hit by player");
                if (other.gameObject.GetComponent<BikerTheyThemController>().checkSpeedEffectThreshold())
                {
                    stateMachine.ChangeState(new GhostDyingState(this, stateMachine));
                }
            }
        }
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

    public float attack_cooldown = 5f;
    public float attack_cooldown_timer;
    public bool can_attack;

    public override void Enter()
    {

        Debug.Log("Ghost is chasing");

        attack_cooldown_timer = 3f;
        can_attack = false;
    }

    public override void Update()
    {

        if (owner.checkIfVelocityIsForward(GameManager.Instance.GetPlayer().GetComponent<Rigidbody>()))
        {
            owner.gameObject.transform.position = Vector3.MoveTowards(
                owner.gameObject.transform.position,
                GameManager.Instance.GetPlayer().transform.position + Vector3.up * 1f + GameManager.Instance.GetPlayer().GetComponent<Rigidbody>().linearVelocity * 2f,
                0.01f);
        }
        else
        {
            owner.gameObject.transform.position = Vector3.MoveTowards(
                owner.gameObject.transform.position,
                GameManager.Instance.GetPlayer().transform.position,
                0.01f);
        }

        if ((GameManager.Instance.GetPlayer().transform.position - owner.transform.position).magnitude < 7f && can_attack)
        {
            stateMachine.ChangeState(new GhostAttackingState(owner, stateMachine));
        }

        else if ((GameManager.Instance.GetPlayer().transform.position - owner.transform.position).magnitude > 15f)
        {
            stateMachine.ChangeState(new GhostIdleState(owner, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        if (!can_attack)
        {
            attack_cooldown_timer += Time.fixedDeltaTime;
            if (attack_cooldown_timer >= attack_cooldown)
            {
                can_attack = true;
                attack_cooldown_timer = 0f;
            }
        }
    }
}



public class GhostAttackingState : MonsterAttackingState<Ghost>
{
    public GhostAttackingState(Ghost owner, StateMachine<Ghost> sm) : base(owner, sm) { }
    public Coroutine dashCoroutine;
    public bool is_projecting;

    public override void Enter()
    {
        Debug.Log("Ghost is attacking");
        is_projecting = true;
        dashCoroutine = owner.StartCoroutine(DashAtPlayer());
    }

    IEnumerator DashAtPlayer()
    {

        yield return new WaitForSeconds(1f);

        is_projecting = false;

        Vector3 direction = (GameManager.Instance.GetPlayer().transform.position - owner.transform.position).normalized;

        float dashSpeed = 20f;
        float dashDuration = 0.4f;
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            owner.transform.position += direction * dashSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        //Debug.Log("Done dashong" + Time.time);

        stateMachine.ChangeState(new GhostIdleState(owner, stateMachine));
    }
}



public class GhostDyingState : MonsterDyingState<Ghost>
{
    public GhostDyingState(Ghost owner, StateMachine<Ghost> sm) : base(owner, sm) { }

    
    public override void Enter()
    {
        Debug.Log("Ghost is dying");
        UnityEngine.Object.Destroy(owner.gameObject);
    }

}