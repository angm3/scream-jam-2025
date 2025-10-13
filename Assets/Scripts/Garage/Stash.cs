using System;
using System.Collections;

[Serializable]
public class Stash
{
    public int candyCount;
    public ArrayList blueprints = new ArrayList();
    public ArrayList potionIngredients = new ArrayList();

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
            blueprints.Add(blueprint);
            playerInventory.RemoveItemFromInventory(blueprint);
        }
    }

    public void StashIngredient(Inventory playerInventory, Collectible ingredient)
    {
        if (playerInventory.potionIngredients.Contains(ingredient))
        {
            potionIngredients.Add(ingredient);
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
        if (blueprints.Contains(blueprint))
        {
            blueprints.Remove(blueprint);
            playerInventory.blueprints.Add(blueprint);
        }
    }

    public void WithdrawIngredient(Inventory playerInventory, Collectible ingredient)
    {
        if (potionIngredients.Contains(ingredient))
        {
            potionIngredients.Remove(ingredient);
            playerInventory.potionIngredients.Add(ingredient);
        }
    }
}