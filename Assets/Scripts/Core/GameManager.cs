using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private GameObject player_ref;
    //public GameObject camera_ref;
    public static GameManager Instance { get; private set; }

    public Stash stash;

    public Inventory currentPlayerInventory;
    public List<string> equippedBikeUpgrades = new List<string>();
    public Texture2D cursorCrosshair;
    public Vector2 hotSpot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    //private Transform playerTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes

            // Set up stash and inventory 
            stash = new Stash();
            currentPlayerInventory = new Inventory();

            Cursor.SetCursor(cursorCrosshair, hotSpot, cursorMode);

            // TESTING
            //currentPlayerInventory.candyCount = 6;
            //stash.candyCount = 118;
        }
    }

    private void Start()
    {
        SceneManager.LoadScene("UI", LoadSceneMode.Additive);
    }

    public void HandleSuccessfulExtraction()
    {
        Debug.Log("Extraction Successful");
        ReturnToGarage();
    }
    
    public void ReturnToGarage()
    {
        SceneController.Instance.LoadScene("Garage", includeUI: true);
    }

    // Called when starting a new run from garage
    public void StartNewRun()
    {
        // For now, just reset inventory candy to 0
        // Later: transfer candy from stash to inventory based on UI selection

        Debug.Log("New run started!");
        Debug.Log("Playing run music");
        if (GeneralInfo.Instance != null)
        {
            GeneralInfo.Instance.GetReferenceToText();
        }
        AudioManager.Instance.PlayMusic("music_run");
        
        for (int i = 0; i < equippedBikeUpgrades.Count; i++)
        {
            Debug.Log($"Equipped upgrade: {equippedBikeUpgrades[i]}");
        }
    }

    // Add your game management logic here
    public void StartGame()
    {
        Debug.Log("Game Started!");
        // ...
    }

    public void EndGame()
    {
        Debug.Log("Game Over!");
        Application.Quit();
    }

    // Called by player when scene loads
    public void RegisterPlayer(GameObject player)
    {
        player_ref = player;
    }

    public void TransferInventoryToStash()
    {
        Debug.Log("Transfer inventory to stash");
        if (player_ref == null)
        {
            Debug.LogWarning("No player registered, cannot transfer inventory");
            return;
        }

        Inventory playerInventory = player_ref.GetComponent<BikerTheyThemController>().inventory;
        if (playerInventory == null)
        {
            Debug.LogWarning("Player has no Inventory component");
            return;
        }

        // Transfer all candy
        stash.StashItems(playerInventory, playerInventory.candyCount);

        // Transfer all blueprints
        while (playerInventory.blueprints.Count > 0)
        {
            Collectible blueprint = (Collectible)playerInventory.blueprints[0];
            stash.StashBlueprint(playerInventory, blueprint);
        }

        // Transfer all potion ingredients
        while (playerInventory.potionIngredients.Count > 0)
        {
            Collectible ingredient = (Collectible)playerInventory.potionIngredients[0];
            stash.StashIngredient(playerInventory, ingredient);
        }

        Debug.Log($"Transferred inventory to stash. Stash now has {stash.candyCount} candy, {stash.items.Count} blueprints, {stash.ingredients.Count} ingredients");
        
    }

    public GameObject GetPlayer() => player_ref;
}