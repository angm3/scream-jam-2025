using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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

    [Header("Properties")]
    [SerializeField] public int inventorySize = 15;
    [SerializeField] public int stashSize = 108;
    [SerializeField] public int ingredientsSize = 1;
    [SerializeField] public int bikeUpgradesSize = 3;



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
        CreateEmptySlots(bikeUpgradesGridContainer, bikeUpgradesSize);
        // TODO, hard coded for now in the editor
        //CreateEmptySlots(ingredientsGridContainer, ingredientsSize);

        LoadPlayerInventory();
        LoadStash();
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
    }

    void SyncUIToData()
    {
        Inventory playerInv = GameManager.Instance.currentPlayerInventory;
        Stash stash = GameManager.Instance.stash;

        Debug.Log($"BEFORE sync - inv candy: {playerInv.candyCount}, stash candy: {stash.candyCount}");

        playerInv.candyCount = 0;
        stash.candyCount = 0;
        // TODO clear other stuff

        // Read from Inventory grid
        foreach (Transform slot in inventoryGridContainer)
        {
            foreach (Transform child in slot)
            { 
                CollectibleUIItem item = child.GetComponent<CollectibleUIItem>();
                if (item != null && item.collectibleType == "candy")
                {
                    playerInv.candyCount += item.stackCount;
                }

                // TODO: add items and ingredient handling
            }
        }

        // Read from Stash grid
        foreach (Transform slot in stashGridContainer)
        {
            foreach (Transform child in slot)
            {
                CollectibleUIItem item = child.GetComponent<CollectibleUIItem>();
                if (item != null && item.collectibleType == "candy")
                { 
                    stash.candyCount += item.stackCount;
                }

                // TODO: add items and ingredient handling
            }
        }

        Debug.Log($"AFTER sync - inv candy: {playerInv.candyCount}, stash candy: {stash.candyCount}");
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

    public void OnStartRunClicked()
    {
        Debug.Log("Button clicked");

        // Temporarily do the sync from ui here
        // later we'll want to do it when we close the stash screen to interact with elements in the garage scene
        SyncUIToData();

        GameManager.Instance.StartNewRun();
        SceneController.Instance.LoadScene("SampleScene", includeUI: true);
    }
}

