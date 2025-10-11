using System;
using System.Collections;
using UnityEngine;

public class Inventory
{
    public int candyCount;
    public ArrayList blueprints;
    public ArrayList potionIngredients;


    public Inventory()
    {
        this.candyCount = 0;
        this.blueprints = new ArrayList();
        this.potionIngredients = new ArrayList();
    }

    public void AddToInventory(Collectible item)
    {
        switch (item.type) {
            case "candy":
                this.candyCount++;
                break;
            case "blueprint":
                this.blueprints.Add(item);
                break;
            case "potion_ingredient":
                this.potionIngredients.Add(item);
                break;

        }

    }

    public void RemoveFromInventory(Collectible item)
    {
        switch (item.type)
        {
            case "candy":
                this.candyCount--;
                break;
            case "blueprint":
                this.blueprints.Remove(item);
                break;
            case "potion_ingredient":
                this.potionIngredients.Remove(item);
                break;

        }
    }

    public void DropInventory()
    {
        this.candyCount = 0;
        this.blueprints.Clear();
        this.potionIngredients.Clear();
    }

    public void PickUpInventory(Inventory droppedInventory)
    {
        this.candyCount += droppedInventory.candyCount;
        this.blueprints.AddRange(droppedInventory.blueprints);
        this.potionIngredients.AddRange(droppedInventory.potionIngredients);
    }

}
