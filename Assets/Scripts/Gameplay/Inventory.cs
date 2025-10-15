using System;
using System.Collections;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public int candyCount;
    public int maxCandyCount;
    public ArrayList blueprints;
    public ArrayList potionIngredients;

    public Inventory()
    {
        candyCount = 0;
        blueprints = new ArrayList();
        potionIngredients = new ArrayList();
        maxCandyCount = 15; // tweak as needed
        EventBus.Publish(new InventoryChangedEvent());
    }

    public void AddToInventory(Collectible item)
    {
        switch (item.type) {
            case "candy":
                candyCount++;
                break;
            case "blueprint":
                blueprints.Add(item.id);
                break;
            case "potion_ingredient":
                potionIngredients.Add(item);
                break;

        }
        EventBus.Publish(new InventoryChangedEvent());
    }

    public void RemoveCandyFromInventory(int count)
    {
        if (candyCount >= count)
        {
            candyCount -= count;
        }
        else
        {
            Debug.LogWarning("trying to remove too much candy!");
        }
        EventBus.Publish(new InventoryChangedEvent());
    }
    
    public void RemoveItemFromInventory(Collectible item)
    {
        switch (item.type)
        {
            case "candy": // shouldn't happen
                candyCount--;
                break;
            case "blueprint":
                blueprints.Remove(item);
                break;
            case "potion_ingredient":
                potionIngredients.Remove(item);
                break;

        }
        EventBus.Publish(new InventoryChangedEvent());
    }

    public void DropInventory()
    {
        candyCount = 0;
        blueprints.Clear();
        potionIngredients.Clear();
        EventBus.Publish(new InventoryChangedEvent());
        Debug.Log("Dropped inventory");
    }

    public void PickUpInventory(Inventory droppedInventory)
    {
        candyCount += droppedInventory.candyCount;
        blueprints.AddRange(droppedInventory.blueprints);
        potionIngredients.AddRange(droppedInventory.potionIngredients);
        EventBus.Publish(new InventoryChangedEvent());
    }

}
