using System;
using System.Collections;

[Serializable]
public class Stash
{
    public int candyCount;
    public int maxCandyStackSize = 5;
    public ArrayList items = new ArrayList();
    public ArrayList ingredients = new ArrayList();

    // Transfer FROM player TO stash
    public void StashItems(Inventory playerInventory, int candyToStash)
    {
        if (candyToStash <= playerInventory.candyCount)
        {
            candyCount += candyToStash;
            playerInventory.RemoveCandyFromInventory(candyToStash);
        }
    }

    public void StashBlueprint(Inventory playerInventory, Collectible blueprint)
    {
        if (playerInventory.blueprints.Contains(blueprint))
        {
            items.Add(blueprint);
            playerInventory.RemoveItemFromInventory(blueprint);
        }
    }

    public void StashIngredient(Inventory playerInventory, Collectible ingredient)
    {
        if (playerInventory.potionIngredients.Contains(ingredient))
        {
            ingredients.Add(ingredient);
            playerInventory.RemoveItemFromInventory(ingredient);
        }
    }

    // Transfer FROM stash TO player
    public void WithdrawCandy(Inventory playerInventory, int candyToWithdraw)
    {
        if (candyToWithdraw <= candyCount)
        {
            candyCount -= candyToWithdraw;
            playerInventory.candyCount += candyToWithdraw;
            //playerInventory.UpdateCounterText();
        }
    }

    public void WithdrawBlueprint(Inventory playerInventory, Collectible blueprint)
    {
        if (items.Contains(blueprint))
        {
            items.Remove(blueprint);
            playerInventory.blueprints.Add(blueprint);
        }
    }

    public void WithdrawIngredient(Inventory playerInventory, Collectible ingredient)
    {
        if (ingredients.Contains(ingredient))
        {
            ingredients.Remove(ingredient);
            playerInventory.potionIngredients.Add(ingredient);
        }
    }
}