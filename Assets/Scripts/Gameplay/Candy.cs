using UnityEngine;

public class Candy : Collectible
{

    public Candy()
    {
        this.type = "candy";
    }

    public override void OnTriggerEnter(Collider other)
    {
        // validate other is the player using 'player' tag
        Debug.Log("Candy trigger");
        stateMachine.ChangeState(new CollectibleCollectedState(this, stateMachine));
    }
}
