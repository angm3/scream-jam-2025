using UnityEngine;

public class BikerTheyThemController : MonoBehaviour
{
    public StateMachine<BikerTheyThemController> stateMachine;

    public Inventory inventory;
    
    private float maxSpeed = 11f; // tweak as needed
    public Rigidbody rb;

    public BikerTheyThemController()
    { 
        stateMachine = new StateMachine<BikerTheyThemController> ();
        // set initial state
        stateMachine.ChangeState(new IdleState(this, stateMachine));
        inventory = new Inventory();
    }

    void OnEnable()
    {
        EventBus.Subscribe<PlayerDamageEvent>(TakeDamage);
        EventBus.Subscribe<PlayerAddInventoryEvent>(GetCollectible);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<PlayerDamageEvent>(TakeDamage);
        EventBus.Unsubscribe<PlayerAddInventoryEvent>(GetCollectible);

    }


    void TakeDamage(PlayerDamageEvent e) {
        Debug.Log("Took damage: " + e.playerDamage.ToString());
    }

    void GetCollectible(PlayerAddInventoryEvent e)
    {
        Debug.Log("Adding to inventory: " + e.item.type.ToString());
        this.inventory.AddToInventory(e.item);
        // TODO: on death clear inventory
    }

    private void Start()
    {

        rb = GetComponent<Rigidbody>();

        //speed = 0f;
        GameManager.Instance?.RegisterPlayer(this.gameObject);
    }


    public void HandleMovement()
    {
        // if W is pressed, accelerate
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * 22f, ForceMode.Acceleration);
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
            // rotate bike left visually 
            transform.Rotate(0, 0, 2f);
        }

        // if D is pressed, turn right
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * 10f, ForceMode.Acceleration);
            transform.Rotate(0, 0, -2f);
        }

        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
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

                // still point foward if velocity is negative
                if (Vector3.Dot(transform.forward, flatVel) < 0)
                    targetRot = Quaternion.LookRotation(-flatVel.normalized, Vector3.up);

                // Smoothly rotate toward that direction
                float rotationSpeed = 6f;
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }
        }
    }
    
    public void HandleMovementPerFrame()
    {
         if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jump!");
            rb.AddForce(-Physics.gravity * 40f, ForceMode.Acceleration);
        }
    }
    
        

    // Update is called once per frame
    void Update()
    {
        HandleMovementPerFrame();
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
