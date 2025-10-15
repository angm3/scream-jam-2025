using UnityEngine;

public class BikerTheyThemController : MonoBehaviour
{
    public StateMachine<BikerTheyThemController> stateMachine;
    public GameObject tombstoneRef;
    public Inventory inventory;
    public Sugar playerSugar;
    private float candyHealthIncrease = 20; // tweak
    public Rigidbody rb;

    // Bike Part Transforms
    public Transform frontWheel;
    public Transform backWheel;
    public Transform pedals;
    public Transform handlebars;

    // Bike parameters (all made up)
    float wheelRadius = 0.25f;
    float wheelCircumference; 
    float gearRatio = 4f;
    float desiredSteerAngle = 0f;
    float steerAngle = 0f;
    Quaternion handlebarsInitialRot;
    Quaternion frontWheelInitialRot;
    Vector3 dirHBFromFW;

    // Speed thresholds
    private float maxSpeed = 20f;               // tweak as needed
    private float minSpeedForTurn = 0.5f;
    private float minSpeedForDrift = 1.5f;
    private float speedEffectThreshold = 12f;   // threshold for effects and bike ramming damage
    private float idleStateResetSpeed = 0.2f;

    // Force parameters
    float forward_force = 10f;
    float brake_force = 10f;
    float backward_force = 6f;
    float turn_force = 12f;
    float drift_force = 0.4f; 

    // Turn Parameters
    float forward_turn_roll = 3f;
    float forward_turn_yaw = 1f;
    float backward_turn_roll = 3f;
    float backward_turn_yaw = 1f;

    // Drift Parameters
    float drift_forward_roll = 0.5f;    // Drift roll applied per frame
    float drift_forward_yaw = 4f;       // Drift yaw applied per frame
    float max_drift_yaw = 360f;         // Maximal yaw change angle allowed over the course of a drift
    float post_drift_timer = 0f;        // Time initialized to zero
    float post_drift_time_lock = 0.2f;  // Time to "lock" player from turning left or right after a drift stops...a small time lock helps with stability
    int drift_rotate_counter = 0;       // Counter that ensures the bike is not rotated beyond some yaw limit
    Vector3 forwardAtStartOfDrift = Vector3.zero;

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

    void Awake()
    {
        GameManager.Instance.RegisterPlayer(this.gameObject);
        this.gameObject.tag = "Player";
    }

    private void Start()
    {
        // Use the GameManager to get the player's inventory
        inventory = GameManager.Instance.currentPlayerInventory;
        playerSugar = FindFirstObjectByType<Sugar>();
        rb = GetComponent<Rigidbody>();

        frontWheel = transform.Find("bike/FrontWheel");
        backWheel = transform.Find("bike/BackWheel");
        pedals = transform.Find("bike/Pedals");
        handlebars = transform.Find("bike/Handlebars");

        handlebarsInitialRot = handlebars.localRotation;
        frontWheelInitialRot = frontWheel.localRotation;
        dirHBFromFW = frontWheel.InverseTransformDirection(handlebars.position - frontWheel.position);
        wheelCircumference = 2f*Mathf.PI*wheelRadius;
        
        // Initialize jump timer
        jump_timer = 0;
        jump_cooldown_timer = jump_cooldown;

        // Set Physical Material
        PhysicsMaterial tireFriction = new PhysicsMaterial();
        tireFriction.dynamicFriction = 0.12f;
        tireFriction.staticFriction = 0.2f;
        tireFriction.frictionCombine = PhysicsMaterialCombine.Multiply;

        // Assign the material to the bike's collider
        Collider collider = GetComponent<BoxCollider>();
        if (collider != null)
        {
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
            if (stateMachine.CurrentState is DriftingState)
            {
                resetVelocityAtEndOfDrift();
            }
            stateMachine.ChangeState(new AcceleratingState(this, stateMachine));
        }
        
        // Force a state reset to idle if the bike slows down too much
        if (rb.linearVelocity.magnitude < idleStateResetSpeed)
        {
            // switch states to idle
            if (stateMachine.CurrentState is DriftingState)
            {
                resetVelocityAtEndOfDrift();
            }
            stateMachine.ChangeState(new IdleState(this, stateMachine));
        }

        // If in drift state AND A and D are not pressed OR S is not pressed, kill the drift and force an idle state
        if (((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) || !Input.GetKey(KeyCode.S)) && stateMachine.CurrentState is DriftingState)
        {
            resetVelocityAtEndOfDrift();
            stateMachine.ChangeState(new IdleState(this, stateMachine));
        }

        // Set current steer angle
        if ((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) || stateMachine.CurrentState is DriftingState)
        {
            desiredSteerAngle = 0;
        }
        else 
        {
            if(Input.GetKey(KeyCode.A))
            {
                
                desiredSteerAngle = checkIfVelocityIsForward() ? -30 : 30;
            }
            if(Input.GetKey(KeyCode.D))
            {
                desiredSteerAngle = checkIfVelocityIsForward() ? 30 : -30;
            }
        }

        BikeYawAnimations(desiredSteerAngle);
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

    public void ConsumeCandy()
    {
        Debug.Log("in ConsumeCandy");
        inventory.RemoveCandyFromInventory(1);
        playerSugar.AddSugar(candyHealthIncrease);
    }

    public bool CanUseCandy()
    {
        return inventory.candyCount > 0;
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
        rb.linearVelocity = transform.forward * Mathf.Max(minSpeedForDrift * 0.6f, rb.linearVelocity.magnitude * 0.8f);
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
        BikeRollAnimations();

        bool meetsThreshold = checkSpeedEffectThreshold();
        if (meetsThreshold)
        {
            Debug.Log("Reached speed effect threshold");
            // TODO: apply light effect here
        } else
        {
            Debug.Log("Player speed below threshold");
        }

        //Debug.Log("Linear Velocity: " + rb.linearVelocity.ToString());
        //Debug.Log("Linear Velocity Euler Angles: " + rb.rotation.eulerAngles.ToString());

    }

    private void LinearMotion() 
    {      
        // if W is pressed, accelerate
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * forward_force, ForceMode.Acceleration);
        }

        // if S is pressed, deccelerate
        if(stateMachine.CurrentState is not DriftingState)
        {
            if (Input.GetKey(KeyCode.S))
            {
                if(checkIfVelocityIsForward())
                {
                    rb.AddForce(-transform.forward * brake_force, ForceMode.Acceleration);
                    Debug.Log("Deccelerating");
                }
                else {
                    rb.AddForce(-transform.forward * backward_force, ForceMode.Acceleration);
                    Debug.Log("Backing Up");
                }
            }
        }
        
    }

    private void LateralMotion() 
    {
        post_drift_timer += Time.fixedDeltaTime;

        // If pressing A or D, enter the below block of code
        if (rb.linearVelocity.magnitude > minSpeedForTurn && ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))))
        {
            // If below the minimum drift speed and in a drifing state, kill the drift and force an idle state
            if (rb.linearVelocity.magnitude < minSpeedForDrift && stateMachine.CurrentState is DriftingState)
            {
                resetVelocityAtEndOfDrift();
                stateMachine.ChangeState(new IdleState(this, stateMachine));
                return;
            }
            
            // If NOT pressing S OR Velocity is backwards and bike is not in a drift, perform normal turning
            if (!Input.GetKey(KeyCode.S) || (!checkIfVelocityIsForward() && stateMachine.CurrentState is not DriftingState))
            {
                if (post_drift_timer > post_drift_time_lock)
                {
                    // if A is pressed, turn left
                    if (Input.GetKey(KeyCode.A))
                    {
                        if (checkIfVelocityIsForward())
                        {
                            rb.AddForce(-transform.right * turn_force, ForceMode.Acceleration);
                            transform.Rotate(0, -forward_turn_yaw, forward_turn_roll);
                        }
                        else
                        {
                            rb.AddForce(transform.right * turn_force, ForceMode.Acceleration);
                            transform.Rotate(0, backward_turn_yaw, -backward_turn_roll);
                        }

                    }

                    // if D is pressed, turn right
                    if (Input.GetKey(KeyCode.D))
                    {
                        if (checkIfVelocityIsForward())
                        {
                            rb.AddForce(transform.right * turn_force, ForceMode.Acceleration);
                            transform.Rotate(0, forward_turn_yaw, -forward_turn_roll);
                        }
                        else
                        {
                            rb.AddForce(-transform.right * turn_force, ForceMode.Acceleration);
                            transform.Rotate(0, -backward_turn_yaw, backward_turn_roll);
                        }
                    }
                }

            }

            // If S is pressed OR bike is in a drifting state AND going backwards 
            else
            {
                // If this is  a new drift state, set it, capture the current forwards vector, and force the bike 
                // to be uprights.
                if (stateMachine.CurrentState is not DriftingState && checkIfVelocityIsForward())
                {
                    stateMachine.ChangeState(new DriftingState(this, stateMachine));
                    transform.Rotate(0f, 0f, -transform.eulerAngles.z); // Force bike to be upright
                    forwardAtStartOfDrift = transform.forward;          // Capture forward direction (drift forces act opposite this)
                    Debug.Log("Drift: New Drift State.");
                    drift_rotate_counter = 0;
                }
                // If the bike is at the minimum drift speed...
                if (rb.linearVelocity.magnitude > minSpeedForDrift)
                {
                    // Drift left...
                    if (Input.GetKey(KeyCode.A))
                    {
                        rb.AddForce(-forwardAtStartOfDrift * drift_force);


                        if (drift_rotate_counter < Mathf.Ceil(max_drift_yaw / drift_forward_yaw))
                        {
                            transform.Rotate(0, -drift_forward_yaw, drift_forward_roll, Space.Self);
                            drift_rotate_counter += 1;

                        }
                    }
                    // Drift right...
                    else if (Input.GetKey(KeyCode.D))
                    {
                        rb.AddForce(-forwardAtStartOfDrift * drift_force);

                        if (drift_rotate_counter < Mathf.Ceil(max_drift_yaw / drift_forward_yaw))
                        {
                            transform.Rotate(0, drift_forward_yaw, -drift_forward_roll, Space.Self);
                            drift_rotate_counter += 1;
                        }

                    }

                    Debug.Log("Drift Euler Angles: " + transform.eulerAngles);
                }
                // If bike is below minimum drifitng speed, kill the drift and force an idle state
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

    private void BikeRollAnimations() {
        // Rotate the front and bike ties and pedals to animate them as rolling
        if(stateMachine.CurrentState is not DriftingState) {
            float distanceThisFrame = rb.linearVelocity.magnitude * Time.fixedDeltaTime;
            float rotations = distanceThisFrame / wheelCircumference;
            float degrees = rotations * 360f;
            if(checkIfVelocityIsForward()) {
                frontWheel.Rotate(-Vector3.forward*degrees);
                backWheel.Rotate(-Vector3.forward*degrees);
                pedals.Rotate(Vector3.right*degrees/gearRatio);
            }
            else {
                frontWheel.Rotate(Vector3.forward*degrees);
                backWheel.Rotate(Vector3.forward*degrees);
                pedals.Rotate(-Vector3.right*degrees/gearRatio);
            }
        }
    }

    private void BikeYawAnimations(float targetAngle) 
    {
        steerAngle = Mathf.Lerp(steerAngle, targetAngle, Time.deltaTime*5f);
        
        // Rotate the handlebars and front wheel
        handlebars.localRotation = handlebarsInitialRot * Quaternion.Euler(Vector3.forward * steerAngle);

        frontWheel.localRotation = frontWheelInitialRot * Quaternion.Euler(dirHBFromFW * steerAngle);
    }
    
    public void HandleMovementPerFrame()
    {
        if (Input.GetKey(KeyCode.P))
        {
            Debug.Log("P hit");
            EventBus.Publish(new EnemyDamageEvent(25));
        }
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