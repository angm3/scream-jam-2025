using UnityEngine;

public class PlayerAddInventoryEvent
{
    // add to inventory based on type received
    public Collectible item;

    public PlayerAddInventoryEvent(Collectible newItem)
    {
        item = newItem;
    }
}
