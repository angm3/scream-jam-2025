using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int candyCount;

    // [SerializeField]
     public TMP_Text candyCounterText;
    public ArrayList blueprints;
    public ArrayList potionIngredients;

    public void Start()
    {
        candyCount = 0;
        blueprints = new ArrayList();
        potionIngredients = new ArrayList();
        UpdateCounterText();
    }

    private void UpdateCounterText()
    {
        Debug.Log($"Updating counter on {gameObject.name}", gameObject);

        if (candyCounterText != null)
        {
            candyCounterText.text = $"Candy: {candyCount}";
        } else
        {
            Debug.Log("Not updating candy counter ", candyCounterText);
        }
    }

    public void AddToInventory(Collectible item)
    {
        switch (item.type) {
            case "candy":
                candyCount++;
                break;
            case "blueprint":
                blueprints.Add(item);
                break;
            case "potion_ingredient":
                potionIngredients.Add(item);
                break;

        }
        UpdateCounterText();
    }

    public void RemoveFromInventory(Collectible item)
    {
        switch (item.type)
        {
            case "candy":
                candyCount--;
                break;
            case "blueprint":
                blueprints.Remove(item);
                break;
            case "potion_ingredient":
                potionIngredients.Remove(item);
                break;

        }
        UpdateCounterText();
    }

    public void DropInventory()
    {
        candyCount = 0;
        blueprints.Clear();
        potionIngredients.Clear();
        UpdateCounterText();
    }

    public void PickUpInventory(Inventory droppedInventory)
    {
        candyCount += droppedInventory.candyCount;
        blueprints.AddRange(droppedInventory.blueprints);
        potionIngredients.AddRange(droppedInventory.potionIngredients);
        UpdateCounterText();
    }

}
