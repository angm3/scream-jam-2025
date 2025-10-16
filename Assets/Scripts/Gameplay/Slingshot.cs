using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public StateMachine<Slingshot> stateMachine;
    public GameObject projectilePrefab;

    public bool enabled;

    void Start()
    {
        stateMachine = new StateMachine<Slingshot>();
        bool hasEnoughInventory = GameManager.Instance.GetPlayer().GetComponent<BikerTheyThemController>().CanUseCandy();
        //Debug.Log("Slingshot: Constructor - hasEnoughInventory=" + hasEnoughInventory.ToString());
        if (hasEnoughInventory)
        {
            stateMachine.ChangeState(new SlingshotCanShootState(this, stateMachine));
        }
        else
        {
            stateMachine.ChangeState(new SlingshotEmptyState(this, stateMachine));
        }
        
        this.enabled = false;
    }

    void Update()
    {
        stateMachine.Update();
    }
    
    public void EnableSlingshot()
    {
        this.enabled = true;
    }
}


public class SlingshotCanShootState : State<Slingshot>
{
    public SlingshotCanShootState(Slingshot slingshot, StateMachine<Slingshot> stateMachine) : base(slingshot, stateMachine) { }

    public override void Enter()
    {
        //Debug.Log("Slingshot is ready to shoot");
    }

    public override void Update()
    {
        if (Input.GetMouseButtonDown(0) && owner.enabled)
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
        //Debug.Log("Slingshot: enter shooting state");
    }
    
    void ShootProjectile(Vector3 dir)
    {
        GameObject projectile = GameObject.Instantiate(owner.projectilePrefab, owner.transform.position, Quaternion.identity);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.linearVelocity = dir * Mathf.Min(30f, 5f + hold_for_timer * 20f);

        rb.angularVelocity = Random.insideUnitSphere * 10f;

        GameManager.Instance.GetPlayer().GetComponent<BikerTheyThemController>().ConsumeCandy();
        owner.stateMachine.ChangeState(new SlingshotCooldownState(owner, stateMachine));
    }

    public override void Update()
    {
        hold_for_timer += Time.deltaTime;

        if (Input.GetMouseButtonUp(0))
        {
            // Shoot
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out RaycastHit hit))
                targetPoint = hit.point;
            else
                targetPoint = ray.GetPoint(20f); // arbitrary distance ahead

            Vector3 dir = (targetPoint - owner.transform.position).normalized;
            ShootProjectile(dir);
        }
    }
}

public class SlingshotEmptyState : State<Slingshot>
{
    public SlingshotEmptyState(Slingshot slingshot, StateMachine<Slingshot> stateMachine) : base(slingshot, stateMachine) { }

    public override void Enter()
    {
        //Debug.Log("Slingshot is empty");
    }

    public override void Update()
    {
        bool hasEnoughInventory = GameManager.Instance.GetPlayer().GetComponent<BikerTheyThemController>().CanUseCandy();
        if (hasEnoughInventory)
        {
            //Debug.Log("Slingshot: Empty Update - has enough inventory");
            owner.stateMachine.ChangeState(new SlingshotCanShootState(owner, stateMachine));
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
        bool hasEnoughInventory = GameManager.Instance.GetPlayer().GetComponent<BikerTheyThemController>().CanUseCandy();
        // Debug.Log("Slingshot: Update - hasEnoughInventory=" + hasEnoughInventory.ToString());
        if (cooldown_time_counter >= cooldown_timer && hasEnoughInventory)
        {
            //Debug.Log("Slingshot: cooldown Update - switching to can shoot state.");
            owner.stateMachine.ChangeState(new SlingshotCanShootState(owner, stateMachine));
        } else if (!hasEnoughInventory) // need cooldown conditions?
        {
            //Debug.Log("Slingshot: cooldown Update - switching to empty state"); 
            owner.stateMachine.ChangeState(new SlingshotEmptyState(owner, stateMachine));
        }
    }
}
