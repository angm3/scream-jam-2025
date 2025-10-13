using UnityEngine;

public class BikerTheyThemController : MonoBehaviour
{
    public StateMachine<BikerTheyThemController> stateMachine;
    public GameObject tombstoneRef;
    public Inventory inventory;
    public Sugar playerSugar;
    private float candyHealthIncrease = 20; // tweak
    public Rigidbody rb;

    // Speed thresholds
    private float maxSpeed = 11f; // tweak as needed
    private float minSpeedForTurn = 0.5f;
    private float minSpeedForDrift = 2f;
    private float speedEffectThreshold = 7f; // threshold for effects and bike ramming damage

    // Drift Parameters
    float drift_multiplier = 0.4f;
    float drift_forward_roll = 0.5f;
    float drift_forward_yaw = 4f;
    float max_drift_yaw = 360f;
    float post_drift_timer = 0f;
    float post_drift_time_lock = 0.2f;
    Vector3 forwardAtStartOfDrift = Vector3.zero;
    int drift_rotate_counter = 0;

    // Jump Parameters/Variables
    public float jump_timer;
    public float jump_cooldown_timer;
    public float jump_cooldown = 1f;
    private float max_jump_multiplier = 40f;
    private float max_jump_timer = 1.5f;
    public bool jump_started = false;
    
    
    

    public BikerTheyThemController()
    { 
        stateMachine = new StateMachine<BikerTheyThemController> ();
        // set initial state
        stateMachine.ChangeState(new IdleState(this, stateMachine));
    }

    void OnEnable()
    {
        EventBus.Subscribe<PlayerDamageEvent>(TakeDamage);
        EventBus.Subscribe<PlayerAddInventoryEvent>(GetCollectible);
        EventBus.Subscribe<PlayerBumpEvent>(BumpPlayer);
        EventBus.Subscribe<PlayerDeathEvent>(PlayerDeath);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<PlayerDamageEvent>(TakeDamage);
        EventBus.Unsubscribe<PlayerAddInventoryEvent>(GetCollectible);
        EventBus.Unsubscribe<PlayerBumpEvent>(BumpPlayer);
        EventBus.Unsubscribe<PlayerDeathEvent>(PlayerDeath);
    }

    private void Start()
    {
        inventory = FindFirstObjectByType<Inventory>();
        playerSugar = FindFirstObjectByType<Sugar>();
        rb = GetComponent<Rigidbody>();

        //speed = 0f;
        GameManager.Instance?.RegisterPlayer(this.gameObject);
        this.gameObject.tag = "Player";

        jump_timer = 0;
        jump_cooldown_timer = jump_cooldown;

        // Set Physical Material
        PhysicsMaterial tireFriction = new PhysicsMaterial();
        tireFriction.dynamicFriction = 0.12f;
        tireFriction.staticFriction = 0.2f;
        tireFriction.frictionCombine = PhysicsMaterialCombine.Multiply;

        // Assign the material to the bike's collider
        Collider collider = GetComponent<BoxCollider>();
        if (collider != null) {
            collider.material = tireFriction;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovementPerFrame();
        stateMachine.Update();

        if (Input.GetKeyDown(KeyCode.E) && inventory.candyCount > 0)
        {
            ConsumeCandy();
        }
        
        if (Input.GetKeyDown(KeyCode.W))
        {
            // switch states to accelerating
            if(stateMachine.CurrentState is DriftingState) {
                resetVelocityAtEndOfDrift();
            }
            stateMachine.ChangeState(new AcceleratingState(this, stateMachine));
        }
    }
    
    void FixedUpdate()
    {
        HandleMovement();
        stateMachine.FixedUpdate();
    }

    void TakeDamage(PlayerDamageEvent e)
    {
        Debug.Log("Took damage: " + e.playerDamage.ToString());
        playerSugar.DrainSugar(e.playerDamage);
    }
    
    void BumpPlayer(PlayerBumpEvent e)
    {
        Debug.Log("Bumped baby!");
        rb.AddForce(e.direction * e.mag, ForceMode.Acceleration);
    }

    void GetCollectible(PlayerAddInventoryEvent e)
    {
        Debug.Log("Adding to inventory: " + e.item.type.ToString());
        inventory.AddToInventory(e.item);
    }

    void ConsumeCandy()
    {
        Debug.Log("in ConsumeCandy");
        inventory.RemoveCandyFromInventory(1);
        playerSugar.AddSugar(candyHealthIncrease);
    }

    void PlayerDeath(PlayerDeathEvent e)
    {
        Debug.Log("On PlayerDeath");
        Instantiate(tombstoneRef.AddComponent<Tombstone>(), transform.position, Quaternion.Euler(90, 0, 0));
        inventory.DropInventory();
        gameObject.SetActive(false);

        SceneController.Instance.LoadScene("UI", true);
    }

    void resetVelocityAtEndOfDrift() {
        Debug.Log("Reset velocity at end of drift.");
        rb.linearVelocity = transform.forward * minSpeedForDrift * 0.4f;
        drift_rotate_counter = 0;
        post_drift_timer = 0f;
    }

    public void HandleMovement()
    {
        LinearMotion();
        LateralMotion();    
        ClampSpeed();
        if(stateMachine.CurrentState is not DriftingState)
        {
            RotateBiker();
        }
        JumpBiker();

        bool meetsThreshold = checkSpeedEffectThreshold();
        if (meetsThreshold)
        {
            Debug.Log("Reached speed effect threshold");
            // TODO: apply light effect here
        } else
        {
            Debug.Log("Player speed below threshold");
        }

        Debug.Log("Linear Velocity: " + rb.linearVelocity.ToString());
        Debug.Log("Linear Velocity Euler Angles: " + rb.rotation.eulerAngles.ToString());

    }

    private void LinearMotion() 
    {      
        // if W is pressed, accelerate
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * 22f, ForceMode.Acceleration);
        }

        // if S is pressed, deccelerate
        if(stateMachine.CurrentState is not DriftingState)
        {
            if (Input.GetKey(KeyCode.S))
            {
                rb.AddForce(-transform.forward * 15f, ForceMode.Acceleration);
                Debug.Log("Decellerating");
            }
        }
        
    }

    private void LateralMotion() 
    {
        post_drift_timer += Time.fixedDeltaTime;

        if (rb.linearVelocity.magnitude > minSpeedForTurn && ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))))
        {
            if(rb.linearVelocity.magnitude < minSpeedForDrift && stateMachine.CurrentState is DriftingState) {
                resetVelocityAtEndOfDrift();
                stateMachine.ChangeState(new IdleState(this, stateMachine));
                return;
            }

            if(!Input.GetKey(KeyCode.S) || (!checkIfVelocityIsForward() && stateMachine.CurrentState is not DriftingState)) {

                if(post_drift_timer > post_drift_time_lock)
                {
                    // if A is pressed, turn left
                    if (Input.GetKey(KeyCode.A))
                    {
                        if (checkIfVelocityIsForward())
                        {
                            rb.AddForce(-transform.right * 10f, ForceMode.Acceleration);
                            transform.Rotate(0, 0, 3f);
                        }
                        else
                        {
                            rb.AddForce(transform.right * 10f, ForceMode.Acceleration);
                            transform.Rotate(0, 0, -3f);
                        }

                    }

                    // if D is pressed, turn right
                    if (Input.GetKey(KeyCode.D))
                    {
                        if (checkIfVelocityIsForward())
                        {
                            rb.AddForce(transform.right * 10f, ForceMode.Acceleration);
                            transform.Rotate(0, 0, -3f);
                        }
                        else
                        {
                            rb.AddForce(-transform.right * 10f, ForceMode.Acceleration);
                            transform.Rotate(0, 0, 3f);
                        }
                    }
                }
                
            }

            // If below max drift speed AND S is pressed (DRIFTING!!!!)
            else {
                if(stateMachine.CurrentState is not DriftingState && checkIfVelocityIsForward()) {
                    stateMachine.ChangeState(new DriftingState(this, stateMachine));
                    transform.Rotate(0f, 0f, -transform.eulerAngles.z);
                    forwardAtStartOfDrift = transform.forward;
                    Debug.Log("Drift: New Drift State.");
                    drift_rotate_counter = 0;
                }
                if(Vector3.Dot(forwardAtStartOfDrift, rb.linearVelocity) > 0) 
                {
                    if (Input.GetKey(KeyCode.A))
                    {   
                        rb.AddForce(-forwardAtStartOfDrift * drift_multiplier);

                        if(drift_rotate_counter < Mathf.Ceil(max_drift_yaw/drift_forward_yaw)) {
                            transform.Rotate(0, -drift_forward_yaw, drift_forward_roll, Space.Self);
                            drift_rotate_counter += 1;
                            
                        }
                    }
                    else if (Input.GetKey(KeyCode.D))
                    {
                        rb.AddForce(-forwardAtStartOfDrift * drift_multiplier);

                        if(drift_rotate_counter < Mathf.Ceil(max_drift_yaw/drift_forward_yaw)) {
                            transform.Rotate(0, drift_forward_yaw, -drift_forward_roll, Space.Self);
                            drift_rotate_counter += 1;
                        }
                        
                    }

                    Debug.Log("Drift Euler Angles: " + transform.eulerAngles);
                }
                else 
                {
                    resetVelocityAtEndOfDrift();
                    stateMachine.ChangeState(new IdleState(this, stateMachine));
                }

            }
                
        }
    }

    private void ClampSpeed() {
        if (checkIfVelocityIsForward())
        { 
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            if (rb.linearVelocity.magnitude > 0.3f * maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * 0.3f * maxSpeed;
            }
        }
    }

    private void RotateBiker() {
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

    private void JumpBiker() {
        jump_cooldown_timer += Time.fixedDeltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            jump_timer += Time.fixedDeltaTime;
        }
    }
    
    public void HandleMovementPerFrame()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (jump_cooldown_timer > jump_cooldown)
            {
                Debug.Log("Jump Start");
                //rb.AddForce(-Physics.gravity * 30f, ForceMode.Acceleration);
                jump_timer = 0;
                jump_started = true;
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (jump_started)
            {
                Debug.Log("Jump End!");
                rb.AddForce(-Physics.gravity * Mathf.Min(Mathf.Max(jump_timer, 0.4f * max_jump_timer) / max_jump_timer, 1f) * max_jump_multiplier, ForceMode.Acceleration);
                //jump_timer = 0;
                jump_started = false;
                jump_cooldown_timer = 0;
            }
        }
    }

    public bool checkIfVelocityIsForward()
    {
        return Vector3.Dot(rb.transform.forward, rb.linearVelocity) > 0;
    }

    public bool checkSpeedEffectThreshold()
    {
        return rb.linearVelocity.magnitude >= speedEffectThreshold;
    }
    
}


public class IdleState : State<BikerTheyThemController>
{
    public IdleState(BikerTheyThemController owner, StateMachine<BikerTheyThemController> sm) : base(owner, sm) { }

    public override void Enter()
    {
        Debug.Log("Drift: Entered idle state");
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
        Debug.Log("Drift: Entered accelerating state");
    }

    public override void Update()
    {
        Debug.Log("Draining sugar: " + owner.playerSugar.decayRate.ToString());
        // drain sugar
        owner.playerSugar.DrainSugar(owner.playerSugar.decayRate * Time.deltaTime);
        if (Input.GetKeyUp(KeyCode.W))
        {
            // switch states to deccelerating
            owner.stateMachine.ChangeState(new IdleState(owner, owner.stateMachine));
        }
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
        // Debug.Log("Entered deccelerating state");
    }

    public override void Update()
    {

    }
    
    
}



public class DriftingState : State<BikerTheyThemController>
{
    public DriftingState(BikerTheyThemController owner, StateMachine<BikerTheyThemController> sm) : base(owner, sm) { }
    
    private float decay_multiplier = 0.5f;

    public override void Enter()
    {
        Debug.Log("Drift: Entered drifting state");
    }

    public override void Update()
    {
        Debug.Log("Draining sugar: " + (owner.playerSugar.decayRate*decay_multiplier).ToString());
        // drain sugar
        owner.playerSugar.DrainSugar(owner.playerSugar.decayRate * decay_multiplier * Time.deltaTime);
        if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            // switch states to deccelerating
            //owner.stateMachine.ChangeState(new IdleState(owner, owner.stateMachine));
        }
    }
    
    public override void FixedUpdate()
    {
        
    }
}