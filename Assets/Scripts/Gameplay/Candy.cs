using UnityEngine;

public class Candy : Collectible
{

    public Candy()
    {
        this.type = "candy";
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Candy trigger");
            stateMachine.ChangeState(new CollectibleCollectedState(this, stateMachine));
        }
    }
}
