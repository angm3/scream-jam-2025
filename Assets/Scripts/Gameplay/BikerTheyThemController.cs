using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class BikerTheyThemController : MonoBehaviour
{
    public StateMachine<BikerTheyThemController> stateMachine;

    [Header("Transforms")]
    public Transform bike;
    public Transform frontWheelPivot;
    public Transform frontTire;
    public Transform frontSpokes;
    public Transform rearWheelPivot;
    public Transform rearTire;
    public Transform rearSpokes;

    [Header("Rigid Body Parameters")]
    public float pedalTorque = 500f;
    public float brakeForce = 200f;
    public float maxSteeringAngle = 20f;
    public float wheelRadius = 0.25f;
    public float steerStrength = 60.0f; // tuning value
    public float torqueStrength = 10.0f; // tuning value

    private Rigidbody rbBike;
    private float steeringInput;
    private float throttleInput;
    private float brakeInput;
    private float steerAngle;

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

        // Find objects to reference for the transforms
        bike = transform.Find("Bike");
        frontWheelPivot = transform.Find("Bike/FrontWheelPivot");
        frontTire = transform.Find("Bike/FrontWheelPivot/FrontTire");
        frontSpokes = transform.Find("Bike/FrontWheelPivot/FrontWheelSpokes");
        rearWheelPivot = transform.Find("Bike/RearWheelPivot");
        rearTire = transform.Find("Bike/RearWheelPivot/RearTire");
        rearSpokes = transform.Find("Bike/RearWheelPivot/RearWheelSpokes");

        // Initialize Rigid Body
        rbBike = GetComponent<Rigidbody>();
        //Debug.Log("Bike COM: " + rbBike.centerOfMass.ToString());
        rbBike.centerOfMass = rbBike.centerOfMass + new Vector3(0, -0.2f, 0);     // Lower this for stability

        frontWheelPivot.transform.parent = rbBike.transform;

        Debug.Log("Controller on: " + gameObject.name);
        Debug.Log("Rigidbody attached to: " + rbBike.gameObject.name);

        Debug.Log("Bike on: " + (bike ? bike.name : "NULL"));
        Debug.Log("Front tire on: " + (frontTire ? frontTire.name : "NULL"));
        Debug.Log("Front spokes on: " + (frontSpokes ? frontSpokes.name : "NULL"));
        Debug.Log("Rear tire on: " + (rearTire ? rearTire.name : "NULL"));
        Debug.Log("Rear spokes on: " + (rearSpokes ? rearSpokes.name : "NULL"));
        Debug.Log("Rigidbody on: " + rbBike.gameObject.name);

    }

    // Update is called once per frame
    void Update()
    {
        //stateMachine.Update();
        steeringInput = Input.GetAxis("Horizontal");   // 'A'=-1, 'D'=1
        throttleInput = Input.GetAxis("Vertical") > 0 ? Input.GetAxis("Vertical") : 0f;     // 'W'=1
        brakeInput = Input.GetAxis("Vertical") < 0 ? -Input.GetAxis("Vertical") : 0f;        // 'S'=-1

        //Debug.Log("RB Pose: " + rb.position.ToString());
    }

    // Fixed Update is called once per timestamp
    private void FixedUpdate()
    {
        steerAngle = steeringInput * maxSteeringAngle; 

        applyThrottle();
        applySteering();
        applySteeringForces();
        //applySteeringRotation();
        applyBraking();
        //erupdateBikeModelVisuals();
    }

    void applyThrottle()
    {
        //Debug.Log("Throttle Input = " + throttleInput);
        // Calculate the force applied to the rear tire by the throttle/pedal input
        Vector3 force = rbBike.transform.forward * throttleInput * pedalTorque;

        //Debug.Log("Throttle Force: " + force.ToString());

        rbBike.AddForceAtPosition(force, rearWheelPivot.position);

        // Apply the force at the center of the rear wheel
        /*
        if (rearTire != null)
        {
            Debug.Log("Adding Force at wheels Position: " + rearSpokes.position.ToString());
            rbBike.AddForceAtPosition(force, rearTire.position);
        }
        else
        {
            Debug.Log("Adding Force at COM.");
            rbBike.AddForce(force);
        }
        */
        
    }

    void applySteering()
    {
        // Calculate the rotation in the local frame
        Quaternion pivot_rotation = Quaternion.Euler(0f, steerAngle, 0f);
        Debug.Log("Local Rotation:" + pivot_rotation.eulerAngles);

        // Apply the steer to the front tire and spokes
        frontWheelPivot.localRotation = pivot_rotation;
    }

    void applySteeringForces()
    {
        // Get forward velocity and speed magnitude
        Vector3 velocity = rbBike.linearVelocity;
        float speed = velocity.magnitude;

        // Return if below a given velocity
        if (speed < 0.1f)
        {
            return;
        }

        // Compute a yaw direction vector from the bike's forward direction
        Quaternion q = Quaternion.Euler(0f, steerAngle, 0f);
        Vector3 turnDir = q * rbBike.transform.forward;

        Vector3 frontForward = turnDir.normalized;

        Vector3 desiredTurn = (frontForward - rbBike.transform.forward).normalized;

        Vector3 lateralForce = desiredTurn * (speed * steerStrength);
        //lateralForce = rbBike.transform.InverseTransformDirection(lateralForce);

        // Calculate the target velocity direction based on steering
        //Vector3 desiredDir = Vector3.Lerp(bike.transform.forward, turnDir, Mathf.Abs(steeringInput));

        // Compute a force that nudges velocity toward that desired direction
        // Vector3 desiredVelocity = turnDir * speed;
        //Vector3 steerForce = (desiredVelocity - velocity) * 400f; // steering response gain

        //rbBike.AddForceAtPosition(steerForce, frontWheelPivot.position);
        //rbBike.AddForceAtPosition(lateralForce, frontWheelPivot.position, ForceMode.Force);
        //rbBike.AddForce(lateralForce, ForceMode.Force);
        Vector3 torque = steerAngle < 0 ? -Vector3.up * lateralForce.magnitude * torqueStrength : Vector3.up * lateralForce.magnitude * torqueStrength;
        rbBike.AddTorque(torque);

        /*
        Debug.Log("Desired Velocity: " + desiredVelocity.ToString());
        Debug.Log("Velocity: " + velocity.ToString());
        */
        Debug.Log("Steer Force applied at: " + frontWheelPivot.position);
        Debug.Log("Steer Force: " + lateralForce);

        //applySteeringRotation(steerForce);
    }

    void applySteeringRotation(Vector3 steerForce)
    {
        float steerStrength = 1.0f; // tuning value
        //float speedFactor = Mathf.Clamp01(rbBike.linearVelocity.magnitude / max_velocity); // more turn at higher speed

        // Calculate target yaw torque
        //float steerTorque = steerAngle * Mathf.Deg2Rad * steerStrength * speedFactor;

        //Vector3 torque = -Vector3.up * steerForce.magnitude * steerStrength;
        //Vector3 r = frontWheelPivot.position - bike.position;
        Vector3 r = new Vector3(0f, 0f, 0.5f);
        Vector3 torque = -Vector3.Project(Vector3.Cross(bike.InverseTransformDirection(steerForce) , r), Vector3.up);
        // Apply torque about the up axis (Y)
        Debug.Log("Lever Arm: " + r.ToString());
        Debug.Log("Torque: " + torque.ToString());

        rbBike.AddRelativeTorque(torque, ForceMode.Force);
    }

    void applyBraking()
    {
        //Debug.Log("BrakeInput = " + brakeInput);

        // Calculate the force applied to the rear tire by the throttle/pedal input
        Vector3 force = -rbBike.transform.forward * brakeInput * brakeForce;

        //Debug.Log("Brake Force: " + force.ToString());

        // Apply the force at the center of the rear wheel
        //rbBike.AddForceAtPosition(force, rearWheelPivot.position);
        rbBike.AddForceAtPosition(force, rearWheelPivot.position);
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

