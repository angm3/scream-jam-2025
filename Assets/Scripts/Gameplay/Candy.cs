using UnityEngine;

public class Candy : Collectible
{
    public Candy()
    {
        type = "candy";
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Candy trigger");
            stateMachine.ChangeState(new CollectibleCollectedState(this, stateMachine));
        }
    }
}
