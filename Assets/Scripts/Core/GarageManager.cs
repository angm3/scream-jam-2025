using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GarageManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startRunButton;

    // Inventory and Stash UI
    [Header("Stash/Inventory Grids")]
    public GameObject inventorySlotItemPrefab;
    public GameObject emptySlotPrefab;
    public Transform inventoryGridContainer;
    public Transform stashGridContainer;
    public Transform ingredientsGridContainer;
    public Transform bikeUpgradesGridContainer;

    [Header("Sprites")]
    public Sprite candySprite;
    public List<SpriteMapping> blueprintSprites;
    public List<SpriteMapping> ingredientSprites;

    [Header("Properties")]
    [SerializeField] public int inventorySize = 15;
    [SerializeField] public int stashSize = 108;
    [SerializeField] public int ingredientsSize = 1;
    [SerializeField] public int bikeUpgradesSize = 3;

    public List<string> requiredIngredients = new List<string> { "batwing" };
    public List<InventorySlot> ingredientSlots; // drag all ingredient slots here

    private bool hasAllIngredients = false;

    [System.Serializable]
    public class SpriteMapping
    {
        public string id;
        public Sprite sprite;
    }

    // Data to UI
    // - LoadPlayerInventory
    // - LoadStash

    // UI To Data
    // - SyncUIToData (reads ui grid and writes back to inventory and stash in GameManager)


    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (startRunButton != null)
        {
            startRunButton.onClick.AddListener(OnStartRunClicked);
        }

        CreateEmptySlots(inventoryGridContainer, inventorySize);
        CreateEmptySlots(stashGridContainer, stashSize);
        //CreateEmptySlots(bikeUpgradesGridContainer, bikeUpgradesSize);
        // TODO, hard coded for now in the editor
        //CreateEmptySlots(ingredientsGridContainer, ingredientsSize);

        //GameManager.Instance.currentPlayerInventory.candyCount = 12;
        //GameManager.Instance.currentPlayerInventory.blueprints.Add("slingshot");
        //GameManager.Instance.currentPlayerInventory.blueprints.Add("fasttires");
        //GameManager.Instance.currentPlayerInventory.potionIngredients.Add("batwing");

        LoadPlayerInventory();
        LoadEquippedUpgrades();
        LoadStash();
    }

    void Update()
    {
        hasAllIngredients = HasAllIngredients();

        if (hasAllIngredients)
        {
            // TODO you win?
            UIManager.Instance.ShowVictoryScreen();
        }
    }

    Sprite GetSpriteForItem(string type, string id)
    {
        if (type == "candy") return candySprite;

        if (type == "blueprint")
        {
            var mapping = blueprintSprites.Find(x => x.id == id);
            return mapping?.sprite;
        }

        if (type == "potion_ingredient")
        { 
            var mapping = ingredientSprites.Find(x => x.id == id);
            return mapping?.sprite;
        }

        return null;
    }

    void CreateEmptySlots(Transform grid, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Instantiate(emptySlotPrefab, grid);
        }
    }

    void LoadPlayerInventory()
    {
        Inventory playerInv = GameManager.Instance.currentPlayerInventory;

        if (playerInv == null) 
        {
            Debug.LogWarning("No inventory in gamemanager");
            return;
        }

        Debug.Log($"Loading inventory with {playerInv.candyCount} candy");

        if (playerInv.candyCount > 0)
        {
            int remainingCandy = playerInv.candyCount;
            while (remainingCandy > 0)
            {
                int stackAmount = Mathf.Min(remainingCandy, playerInv.maxCandyStackSize);
                CreateUIItem(inventoryGridContainer, "candy", "", stackAmount, candySprite);
                remainingCandy -= stackAmount;
            }
        }

        // TODO items and ingredients

        foreach (var bp in playerInv.blueprints)
        {
            //Collectible bpItem = bp as Collectible;
            //string id = bpItem != null ? bpItem.id : "";
            string id = bp.ToString();
            Debug.LogWarning($"Loading inventory item {id}");
            // dont spawn items that are equipped
            if (GameManager.Instance.equippedBikeUpgrades.Contains(id))
            { 
                continue;
            }

            Sprite sprite = GetSpriteForItem("blueprint", id);
            CreateUIItem(inventoryGridContainer, "blueprint", id, 1, sprite);
        }

        foreach (var ing in playerInv.potionIngredients)
        {
            //Collectible ingItem = ing as Collectible;
            //string id = ingItem != null ? ingItem.id : "";
            string id = ing.ToString();
            Sprite sprite = GetSpriteForItem("potion_ingredient", id);
            CreateUIItem(inventoryGridContainer, "potion_ingredient", id, 1, sprite);
        }
    }

    void LoadStash()
    {
        Stash stash = GameManager.Instance.stash;

        if (stash == null) return;

        if (stash.candyCount > 0)
        {
            int remainingCandy = stash.candyCount;
            while (remainingCandy > 0)
            {
                int stackAmount = Mathf.Min(remainingCandy, stash.maxCandyStackSize);
                CreateUIItem(stashGridContainer, "candy", "", stackAmount, candySprite);
                remainingCandy -= stackAmount;
            }
        }

        // TODO items and ingredients
        foreach (var bp in stash.items)
        {
            //Collectible bpItem = bp as Collectible;
            //string id = bpItem != null ? bpItem.id : "";
            string id = bp.ToString();
            Sprite sprite = GetSpriteForItem("blueprint", id);
            CreateUIItem(stashGridContainer, "blueprint", id, 1, sprite);
        }

        foreach (var ing in stash.ingredients)
        {
            //Collectible ingItem = ing as Collectible;
            //string id = ingItem != null ? ingItem.id : "";
            string id = ing.ToString();
            Sprite sprite = GetSpriteForItem("potion_ingredient", id);
            CreateUIItem(stashGridContainer, "potion_ingredient", id, 1, sprite);
        }
    }

    void LoadEquippedUpgrades()
    {
        foreach (string upgradeId in GameManager.Instance.equippedBikeUpgrades)
        {
            Sprite sprite = GetSpriteForItem("blueprint", upgradeId);
            CreateUIItem(bikeUpgradesGridContainer, "blueprint", upgradeId, 1, sprite);
        }
    }

    void SyncUIToData()
    {
        Inventory playerInv = GameManager.Instance.currentPlayerInventory;
        Stash stash = GameManager.Instance.stash;

        Debug.LogWarning($"BEFORE synced - inv: {playerInv.candyCount} candy, {playerInv.blueprints.Count} blueprints, {playerInv.potionIngredients.Count} ingredients");
        Debug.LogWarning($"BEFORE synced - stash: {stash.candyCount} candy, {stash.items.Count} blueprints, {stash.ingredients.Count} ingredients");

        playerInv.candyCount = 0;
        playerInv.blueprints.Clear();
        playerInv.potionIngredients.Clear();
        stash.candyCount = 0;
        stash.items.Clear();
        stash.ingredients.Clear();

        // Read from Inventory grid
        foreach (Transform slot in inventoryGridContainer)
        {
            foreach (Transform child in slot)
            { 
                //candy
                CollectibleUIItem item = child.GetComponent<CollectibleUIItem>();
                if (item == null) continue;

                if (item != null && item.collectibleType == "candy")
                {
                    playerInv.candyCount += item.stackCount;
                }

                //items
                else if (item.collectibleType == "blueprint")
                {
                    Blueprint bp = new Blueprint(item.collectibleId);
                    playerInv.blueprints.Add(bp);
                }

                //ingredients
                else if (item.collectibleType == "potion_ingredient")
                { 
                    PotionIngredient ing = new PotionIngredient(item.collectibleId);
                    playerInv.potionIngredients.Add(ing);
                }
            }
        }

        // Read from Stash grid
        foreach (Transform slot in stashGridContainer)
        {
            foreach (Transform child in slot)
            {
                //candy
                CollectibleUIItem item = child.GetComponent<CollectibleUIItem>();
                if (item == null) continue;

                if (item != null && item.collectibleType == "candy")
                { 
                    stash.candyCount += item.stackCount;
                }

                //items
                else if (item.collectibleType == "blueprint")
                {
                    Blueprint bp = new Blueprint(item.collectibleId);
                    stash.items.Add(bp);
                }

                //ingredients
                else if (item.collectibleType == "potion_ingredient")
                {
                    PotionIngredient ing = new PotionIngredient(item.collectibleId);
                    stash.ingredients.Add(ing);
                }
            }
        }

        // Read from bike upgrade slots
        // read from bike upgrade slots
        GameManager.Instance.equippedBikeUpgrades.Clear();
        foreach (Transform slot in bikeUpgradesGridContainer)
        {
            foreach (Transform child in slot)
            {
                CollectibleUIItem item = child.GetComponent<CollectibleUIItem>();
                if (item != null)
                {
                    GameManager.Instance.equippedBikeUpgrades.Add(item.collectibleId);
                }
            }
        }

        Debug.LogWarning($"AFTER synced - inv: {playerInv.candyCount} candy, {playerInv.blueprints.Count} blueprints, {playerInv.potionIngredients.Count} ingredients");
        Debug.LogWarning($"AFTER synced - stash: {stash.candyCount} candy, {stash.items.Count} blueprints, {stash.ingredients.Count} ingredients");
    }

    void CreateUIItem(Transform gridContainer, string type, string id, int count, Sprite sprite)
    {
        // Find the first empty slot (one with no children)
        Transform emptySlot = null;
        foreach (Transform child in gridContainer)
        {
            if (child.childCount == 0)
            {
                emptySlot = child;
                break;
            }
        }

        if (emptySlot == null) // I dont think this should happen if the Inventory is making sure its not full
        {
            Debug.Log("No empty slots available");
            return;
        }
        
        GameObject itemUI = Instantiate(inventorySlotItemPrefab, emptySlot);

        CollectibleUIItem item = itemUI.GetComponent<CollectibleUIItem>();
        item.collectibleType = type;
        item.collectibleId = id;
        item.stackCount = count;
        item.itemSpriteImage.sprite = sprite;

        if (item.stackText != null)
        {
            item.stackText.gameObject.SetActive(count > 1);
            item.stackText.text = count.ToString();
        }
    }

    public bool HasAllIngredients()
    {
        foreach (string requiredId in requiredIngredients)
        {
            bool found = false;

            foreach (var slot in ingredientSlots)
            {
                if (slot.slotId != requiredId) continue;

                // check if this slot has an item
                foreach (Transform child in slot.transform)
                {
                    if (child.GetComponent<CollectibleUIItem>() != null)
                    {
                        found = true;
                        break;
                    }
                }

                if (found) break;
            }

            if (!found) return false;
        }

        return true;
    }

    public void OnStartRunClicked()
    {
        Debug.Log("Button clicked");

        // Temporarily do the sync from ui here
        // later we'll want to do it when we close the stash screen to interact with elements in the garage scene
        SyncUIToData();
        SceneController.Instance.LoadScene("SampleScene", includeUI: true);
        GameManager.Instance.StartNewRun();
    }
}

