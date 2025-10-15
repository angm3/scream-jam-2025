using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GarageManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startRunButton;
    [SerializeField] private TextMeshProUGUI counts;
    [SerializeField] private Canvas garageCanvas;
    // private Dictionary<string, GameObject> allPanels = new();
    private List<GameObject> activePanels = new();
    private Dictionary<string, GameObject> allPanels = new();

    private void OnEnable()
    {
        ClickableObject.OnClicked += HandleClickableObjectClick;
    }

    private void OnDisable()
    {
        ClickableObject.OnClicked -= HandleClickableObjectClick;
    }

    // Inventory and Stash UI
    [Header("Stash/Inventory Grids")]
    public GameObject inventorySlotItemPrefab;
    public Transform inventoryGridContainer;
    public Transform stashGridContainer;

    [Header("Sprites")]
    public Sprite candySprite;

    [Header("Properties")]
    [SerializeField] public int inventorySize = 8;
    [SerializeField] public int stashSize = 24;
    

    // Handle creating the appropriate UI from Stash and Inventory

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // note panels on scene
        foreach (Transform child in garageCanvas.transform)
        {
            allPanels[child.name] = child.gameObject;
            Debug.Log("Setting " + child.name + " inactive.");
            child.gameObject.SetActive(false);
        }

        garageCanvas.gameObject.SetActive(false);

        //Stash stash = GameManager.Instance.stash;
        //Inventory inventory = GameManager.Instance.currentPlayerInventory;

        CreateEmptySlots(inventoryGridContainer, inventorySize);
        CreateEmptySlots(stashGridContainer, stashSize);

        LoadPlayerInventory();
        LoadStash();
    }

    void LoadPlayerInventory()
    {
        Inventory playerInv = GameManager.Instance.currentPlayerInventory;

        if (playerInv == null) 
        {
            Debug.LogWarning("No inventory in gamemanager");
        }

        Debug.Log($"Loading inventory with {playerInv.candyCount} candy");

        if (playerInv.candyCount > 0)
        {
            CreateUIItem(inventoryGridContainer, "candy", "", playerInv.candyCount, candySprite);
        }

        // TODO items and ingredients
    }

    void LoadStash()
    {
        Stash stash = GameManager.Instance.stash;

        if (stash == null) return;

        if (stash.candyCount > 0)
        {
            CreateUIItem(stashGridContainer, "candy", "", stash.candyCount, candySprite);
        }

        // TODO items and ingredients
    }

    void CreateUIItem(Transform parent, string type, string id, int count, Sprite sprite)
    {
        Debug.Log("Attempting to create a UI Element");
        GameObject itemUI = Instantiate(inventorySlotItemPrefab, parent);

        CollectibleUIItem item = itemUI.GetComponent<CollectibleUIItem>();
        item.collectibleType = type;
        item.collectibleId = id;
        item.stackCount = count;
        item.iconImage.sprite = sprite;

        if (item.stackText != null)
        {
            item.stackText.gameObject.SetActive(count > 1);
            item.stackText.text = count.ToString();
        }
    }

    void CreateEmptySlots(Transform grid, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject slot = Instantiate(inventorySlotItemPrefab, grid);
            if (slot.GetComponent<CollectibleUIItem>().stackText != null) {
                slot.GetComponent<CollectibleUIItem>().stackText.gameObject.SetActive(false);
            }
        }
    }

    
    private void HandleClickableObjectClick(string objectName)
    {
        Debug.Log($"GarageManager: Click detected on {objectName}");

        List<string> panelsToShow = new();

        // cases for specific objects go here:
        if (objectName.StartsWith("Chest"))
        {
            panelsToShow.Add("InventoryPanel");
            panelsToShow.Add("StashPanel");
        }
        else
        {
            Debug.LogWarning($"No panels defined to show for {objectName}");
        }
        ShowPanels(panelsToShow);
    }

    private void ShowPanels(List<string> panelNames)
    {
        if (panelNames.Count == 0) return;

        garageCanvas.gameObject.SetActive(true);

        // Hide old panels
        foreach (var p in activePanels)
            p.SetActive(false);
        activePanels.Clear();

        // Activate new ones
        foreach (var name in panelNames)
        {
            if (allPanels.TryGetValue(name, out var panel))
            {
                Debug.Log("Setting panel " + panel.name.ToString() + " to active.");
                panel.SetActive(true);
                activePanels.Add(panel);
            } else
            {
                Debug.LogWarning("GarageManager: Panel not found.");
            }
        }
    }

    public void HideAllPanels()
    {
        foreach (var panel in allPanels.Values)
            panel.SetActive(false);

        garageCanvas.gameObject.SetActive(false);
        activePanels.Clear();
    }

    public void OnStartRunClicked()
    {
        Debug.Log("Button clicked");
        GameManager.Instance.StartNewRun();
        SceneController.Instance.LoadScene("SampleScene", includeUI: true);
    }
}

