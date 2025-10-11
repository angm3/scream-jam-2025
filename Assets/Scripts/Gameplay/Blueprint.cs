using UnityEngine;

public class Blueprint : Collectible
{
    public string id;

    public Blueprint(string id)
    {
        this.type = "blueprint";
        this.id = id;
    }

    public override void OnTriggerEnter(Collider other)
    {
        // validate other is the player using 'player' tag
        Debug.Log("blueprint trigger");
        stateMachine.ChangeState(new CollectibleCollectedState(this, stateMachine));
    }
}
