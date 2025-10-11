using UnityEngine;

public class BikerTheyThemController : MonoBehaviour
{
    public StateMachine<BikerTheyThemController> stateMachine;

    public Rigidbody rb;

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

        rb = GetComponent<Rigidbody>();

    }


    public void HandleMovement()
    {
        // if W is pressed, accelerate
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * 25f, ForceMode.Acceleration);
        }

        // if S is pressed, deccelerate
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * 15f, ForceMode.Acceleration);
        }

        // if A is pressed, turn left
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-transform.right * 10f, ForceMode.Acceleration);
        }

        // if D is pressed, turn right
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * 10f, ForceMode.Acceleration);

        }
        
        // Rotate the biker to face the direction of movement
        Vector3 velocity = rb.linearVelocity;

    // Only rotate if we're actually moving
    if (velocity.sqrMagnitude > 0.1f)
    {
        // Flatten the velocity to prevent tilting up/down
        Vector3 flatVel = new Vector3(velocity.x, 0f, velocity.z);

        if (flatVel.sqrMagnitude > 0.001f)
        {
            // Calculate desired rotation based on velocity direction
            Quaternion targetRot = Quaternion.LookRotation(flatVel.normalized, Vector3.up);

            // Smoothly rotate toward that direction
            float rotationSpeed = 8f; // tweak this value for snappier/slower turn
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
    }
        

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }
    
    void FixedUpdate()
    {
        HandleMovement();
        stateMachine.FixedUpdate();
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

    }
}


public class AcceleratingState : State<BikerTheyThemController>
{
    public AcceleratingState(BikerTheyThemController owner, StateMachine<BikerTheyThemController> sm) : base(owner, sm) { }

    public override void Enter()
    {
        Debug.Log("Entered accelerating state");
    }

    public override void Update()
    {
        
    }
    
    public override void FixedUpdate()
    {

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

    }
    
    
}
