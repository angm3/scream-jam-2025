using UnityEngine;

public class Blueprint : Collectible
{
    public string id;

    public Blueprint(string id)
    {
        this.type = "blueprint";
        this.id = id;
    }

    public void Awake()
    {
        Debug.Log("Blueprint Awake");
        stateMachine = new StateMachine<Collectible>();
        stateMachine.ChangeState(new CollectibleIdleState(this, stateMachine));
    }

    public override void OnTriggerEnter(Collider other)
    {
        // validate other is the player using 'player' tag
        Debug.Log("blueprint trigger");
        
        if (other.gameObject.CompareTag("Player"))
        {
            stateMachine.ChangeState(new CollectibleCollectedState(this, stateMachine));
            Collect();
        }
    }
    
    public void Collect()
    {
        Debug.Log(GameManager.Instance.GetPlayer().GetComponent<BikerTheyThemController>().inventory.blueprints[0]);
        // play some particle effects or sound here
        Destroy(gameObject);
    }
}
