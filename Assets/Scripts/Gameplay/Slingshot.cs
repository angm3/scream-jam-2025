
using UnityEngine;


public class Slingshot : MonoBehaviour
{
    public StateMachine<Slingshot> stateMachine;
    public GameObject projectilePrefab;

    public Slingshot()
    {
        stateMachine = new StateMachine<Slingshot>();
        stateMachine.ChangeState(new SlingshotCanShootState(this, stateMachine));
    }
    
    void Update()
    {
        stateMachine.Update();
    }
}


public class SlingshotCanShootState : State<Slingshot>
{
    public SlingshotCanShootState(Slingshot slingshot, StateMachine<Slingshot> stateMachine) : base(slingshot, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Slingshot is ready to shoot");
    }

    public override void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            owner.stateMachine.ChangeState(new SlingshotShootingState(owner, stateMachine));
        }
    }
}

public class SlingshotShootingState : State<Slingshot>
{
    public SlingshotShootingState(Slingshot slingshot, StateMachine<Slingshot> stateMachine) : base(slingshot, stateMachine) { }


    public float max_hold_timer = 1.5f;
    public float hold_for_timer;
    public override void Enter()
    {
        hold_for_timer = 0f;
    }

    public override void Update()
    {
        hold_for_timer += Time.deltaTime;

        if (Input.GetMouseButtonUp(0))
        {
            // Shoot
            Plane playerPlane = new Plane(Vector3.up, owner.transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            Vector3 relativeMousePosition = Vector3.zero;
            if (playerPlane.Raycast(ray, out distance))
            {
                relativeMousePosition = ray.GetPoint(distance);
            }
            //Debug.Log("relativeMousePosition = " + (relativeMousePosition - GameManager.Instance.GetPlayer().transform.position));

            Vector3 baes_projectile_dir = -(GameManager.Instance.GetPlayer().transform.position - relativeMousePosition).normalized;
            // Create projectile
            //GameObject projectilePrefab = Resources.Load<GameObject>("Prefabs/Projectile");
            GameObject projectile = GameObject.Instantiate(owner.projectilePrefab, owner.transform.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody>().linearVelocity = baes_projectile_dir * Mathf.Min(30f, 5f + hold_for_timer * 20f);
            
            // give the projectile some spin randomized
            projectile.GetComponent<Rigidbody>().angularVelocity = new Vector3(
                Random.Range(-10f, 10f),
                Random.Range(-10f, 10f),
                Random.Range(-10f, 10f)
            );
            
            // Recoil player
            //EventBus.Publish(new PlayerBumpEvent(-baes_projectile_dir, Mathf.Min(20f, 5f + hold_for_timer * 15f)));

            owner.stateMachine.ChangeState(new SlingshotCooldownState(owner, stateMachine));
        }
    }
}




public class SlingshotCooldownState : State<Slingshot>
{
    public SlingshotCooldownState(Slingshot slingshot, StateMachine<Slingshot> stateMachine) : base(slingshot, stateMachine) { }
    public float cooldown_timer = 0.75f;
    public float cooldown_time_counter;
    public override void Enter()
    {
        cooldown_time_counter = 0f;
    }
    
    public override void Update()
    {
        cooldown_time_counter += Time.deltaTime;
        if (cooldown_time_counter >= cooldown_timer)
        {
            owner.stateMachine.ChangeState(new SlingshotCanShootState(owner, stateMachine));
        }
    }
}
