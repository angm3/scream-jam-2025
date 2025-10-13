using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    public StateMachine<Collectible> stateMachine;

    public string type; // candy, blueprint, potion_ingredient
                  
    public Collectible()
    {
        stateMachine = new StateMachine<Collectible>();
        // init state is idle
        stateMachine.ChangeState(new CollectibleIdleState(this, stateMachine));
    }

    void Update()
    {
        stateMachine.Update();
    }

    public abstract void OnTriggerEnter(Collider other);

}



public class CollectibleIdleState : State<Collectible>
{
    public CollectibleIdleState(Collectible owner, StateMachine<Collectible> sm) : base(owner, sm) { }

    public override void Enter()
    {
        // Debug.Log("collectible entered idle state");
    }

    public override void Update()
    {
        // bob up and down
        float bobHeight = 0.25f;
        float bobSpeed = 2f;
        owner.transform.position = new Vector3(owner.transform.position.x, owner.transform.position.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight * Time.deltaTime, owner.transform.position.z);
        // rotate slowly
        float rotateSpeed = 45f;
        owner.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }
}

public class CollectibleCollectedState : State<Collectible>
{
    public CollectibleCollectedState(Collectible owner, StateMachine<Collectible> sm) : base(owner, sm) { }

    public override void Enter()
    {
        // on enter collected state, trigger inventory add and destroy self.
        Debug.Log("Collectible collected");
        EventBus.Publish(new PlayerAddInventoryEvent(this.owner));
        //Object.Destroy(this.owner.gameObject);
        
    }
}