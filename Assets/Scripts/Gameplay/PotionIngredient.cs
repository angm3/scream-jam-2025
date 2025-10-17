using UnityEngine;

public class PotionIngredient : Collectible
{
    public string id;

    public PotionIngredient(string id)
    {
        this.type = "potion_ingredient";
        this.id = id;
    }

    public void Awake()
    {
        
        stateMachine = new StateMachine<Collectible>();
        stateMachine.ChangeState(new CollectibleIdleState(this, stateMachine));
    }

    public override void OnTriggerEnter(Collider other)
    {
        // validate other is the player using 'player' tag
        Debug.Log("potion_ingredient trigger");

        if (other.gameObject.CompareTag("Player"))
        {
            stateMachine.ChangeState(new CollectibleCollectedState(this, stateMachine));
            Collect();
        }
    }

    public void Collect()
    {
        Debug.Log(GameManager.Instance.GetPlayer().GetComponent<BikerTheyThemController>().inventory.potionIngredients[0]);
        // play some particle effects or sound here
        GeneralInfo.Instance.SetInfo("Collected Batwing!!!");
        Destroy(gameObject);
    }
}
