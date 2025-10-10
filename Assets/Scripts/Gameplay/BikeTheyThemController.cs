using UnityEngine;

public class BikeTheyThemController : MonoBehaviour
{
    public StateMachine<BikeTheyThemController> stateMachine;

    public BikeTheyThemController()
    { 
        stateMachine = new StateMachine<BikeTheyThemController> ();
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
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }
}


public class IdleState : State<BikeTheyThemController>
{
    public IdleState(BikeTheyThemController owner, StateMachine<BikeTheyThemController> sm) : base(owner, sm) { }

    public override void Enter()
    {
        Debug.Log("Entered idle state");
    }

    public override void Update()
    {
        //Debug.Log("Idling...");

        if (Input.GetKey(KeyCode.W)) {
            //Debug.Log("Key pressed...");
            stateMachine.ChangeState(new MovingState(this.owner, stateMachine));
        }
    }
}


public class MovingState : State<BikeTheyThemController>
{
    public MovingState(BikeTheyThemController owner, StateMachine<BikeTheyThemController> sm) : base(owner, sm) { }

    public override void Enter()
    {
        Debug.Log("Entered moving state");
    }

    public override void Update() {
        //Debug.Log("Moving...");

        Vector3 delta_position = new Vector3(1, 0, 0);
        this.owner.gameObject.transform.position += delta_position * Time.deltaTime;

        if (!Input.GetKey(KeyCode.W))
        {
            stateMachine.ChangeState(new IdleState(this.owner, stateMachine));
        }
    }
}