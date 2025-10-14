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

        Stash stash = GameManager.Instance.stash;

        Debug.Log($"Stash has {stash.candyCount} candy");
        counts.text = "Candy: " + stash.candyCount;
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

        // Tell GameManager to prepare new run
        GameManager.Instance.StartNewRun();

        // Load neighborhood scene
        SceneController.Instance.LoadScene("SampleScene", includeUI: true);
    }
}

