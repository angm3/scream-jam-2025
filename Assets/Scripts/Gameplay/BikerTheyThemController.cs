using UnityEngine;

public class BikerTheyThemController : MonoBehaviour
{
    public StateMachine<BikerTheyThemController> stateMachine;

    public float speed;
    public float theta;
    public float linear_acceleration = 6f;
    public float linear_deceleration = 3f;

    public float max_velocity = 15f;

    public BikerTheyThemController()
    { 
        stateMachine = new StateMachine<BikerTheyThemController> ();
        // set initial state
        stateMachine.ChangeState(new IdleState(this, stateMachine));
    }

    void OnEnable()
    {
        EventBus.Subscribe<PlayerDamageEvent>(TakeDamage);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<PlayerDamageEvent>(TakeDamage);
    }


    void TakeDamage(PlayerDamageEvent e) {
        Debug.Log("Took damage: " + e.playerDamage.ToString());
    }

    private void Start()
    {

        speed = 0f;

    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }
}


public class IdleState : State<BikerTheyThemController>
{
    public IdleState(BikerTheyThemController owner, StateMachine<BikerTheyThemController> sm) : base(owner, sm) { }

    public override void Enter()
    {
        Debug.Log("Entered idle state");
    }

    public override void Update()
    {

        float omega = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            omega = -30f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            omega = 30f;
        }

        float delta_theta = omega * Time.deltaTime;
        this.owner.gameObject.transform.Rotate(0, delta_theta, 0, Space.World);

        this.owner.theta = owner.gameObject.transform.eulerAngles.y * Mathf.PI / 180 + Mathf.PI / 2; ;
        

        Vector3 delta_position = new Vector3(-this.owner.speed * Mathf.Cos(this.owner.theta), 0, this.owner.speed * Mathf.Sin(this.owner.theta));
        this.owner.gameObject.transform.position += delta_position * Time.deltaTime;

        if (this.owner.speed > 0f)
        {
            this.owner.speed -= this.owner.linear_deceleration * Time.deltaTime;
        }
        else if (this.owner.speed < 0f) {
            this.owner.speed += this.owner.linear_deceleration * Time.deltaTime;
        }


        if (Input.GetKey(KeyCode.W)) {
            //Debug.Log("Key pressed...");
            stateMachine.ChangeState(new AcceleratingState(this.owner, stateMachine));
        }

        if (Input.GetKey(KeyCode.S))
        {
            //Debug.Log("Key pressed...");
            stateMachine.ChangeState(new DecceleratingState(this.owner, stateMachine));
        }

    }
}


public class AcceleratingState : State<BikerTheyThemController>
{
    public AcceleratingState(BikerTheyThemController owner, StateMachine<BikerTheyThemController> sm) : base(owner, sm) { }

    public override void Enter()
    {
        Debug.Log("Entered accelerating state");
    }

    public override void Update() {

        this.owner.speed += this.owner.linear_acceleration * Time.deltaTime;

        this.owner.speed = this.owner.speed > this.owner.max_velocity ? this.owner.max_velocity : this.owner.speed;

        float omega = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            omega = -30f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            omega = 30f;
        }

        float delta_theta = omega * Time.deltaTime;
        this.owner.gameObject.transform.Rotate(0, delta_theta, 0, Space.World);

        this.owner.theta = owner.gameObject.transform.eulerAngles.y * Mathf.PI / 180 + Mathf.PI / 2;
        

        Vector3 delta_position = new Vector3(-this.owner.speed * Mathf.Cos(this.owner.theta), 0, this.owner.speed * Mathf.Sin(this.owner.theta));
        this.owner.gameObject.transform.position += delta_position * Time.deltaTime;

        if (this.owner.speed > 0f)
        {
            this.owner.speed -= this.owner.linear_deceleration * Time.deltaTime;
        }
        else if (this.owner.speed < 0f)
        {
            this.owner.speed += this.owner.linear_deceleration * Time.deltaTime;
        }

        if (!Input.GetKey(KeyCode.W))
        {
            stateMachine.ChangeState(new IdleState(this.owner, stateMachine));
        }
    }
}



public class DecceleratingState : State<BikerTheyThemController>
{
    public DecceleratingState(BikerTheyThemController owner, StateMachine<BikerTheyThemController> sm) : base(owner, sm) { }

    public override void Enter()
    {
        Debug.Log("Entered deccelerating state");
    }

    public override void Update()
    {
        this.owner.speed -= this.owner.linear_acceleration * Time.deltaTime;

        float omega = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            omega = 30f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            omega = -30f;
        }

        float delta_theta = omega * Time.deltaTime;
        this.owner.gameObject.transform.Rotate(0, delta_theta, 0, Space.World);

        this.owner.theta = owner.gameObject.transform.eulerAngles.y * Mathf.PI / 180 + Mathf.PI / 2;
        

        Vector3 delta_position = new Vector3(-this.owner.speed * Mathf.Cos(this.owner.theta), 0, this.owner.speed * Mathf.Sin(this.owner.theta));
        this.owner.gameObject.transform.position += delta_position * Time.deltaTime;

        if (this.owner.speed > 0f)
        {
            this.owner.speed -= this.owner.linear_deceleration * Time.deltaTime;
        }
        else if (this.owner.speed < 0f)
        {
            this.owner.speed += this.owner.linear_deceleration * Time.deltaTime;
        }

        if (!Input.GetKey(KeyCode.S))
        {
            stateMachine.ChangeState(new IdleState(this.owner, stateMachine));
        }
    }
}

