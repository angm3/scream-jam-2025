using UnityEngine;

public class Tombstone : MonoBehaviour
{
    public Inventory deathInventory;

    public Tombstone(Inventory droppedInventory)
    {
        deathInventory = droppedInventory;
    }

    private void OnTriggerEnter(Collider other)
    {
        // player ran over tombstone
        // pick up their old inventory and delete tombstone
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<BikerTheyThemController>().inventory.PickUpInventory(deathInventory);
            Object.Destroy(this.gameObject);
        }
    }
}
