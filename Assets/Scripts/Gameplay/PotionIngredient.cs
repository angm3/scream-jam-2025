using UnityEngine;

public class PotionIngredient : Collectible
{
    public string id;

    public PotionIngredient(string id)
    {
        this.type = "potion_ingredient";
        this.id = id;
    }

    public override void OnTriggerEnter(Collider other)
    {
        // validate other is the player using 'player' tag
        Debug.Log("potion_ingredient trigger");
        stateMachine.ChangeState(new CollectibleCollectedState(this, stateMachine));
    }
}
