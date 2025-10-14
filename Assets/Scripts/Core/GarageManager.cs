using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GarageManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startRunButton;
    [SerializeField] private TextMeshProUGUI counts;

    // Inventory UI
    // Stash UI

    // Handle creating the appropriate UI from Stash and Inventory

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Stash stash = GameManager.Instance.stash;

        Debug.Log($"Stash has {stash.candyCount} candy");
        counts.text = "Candy: " + stash.candyCount;
    }

    public void OnStartRunClicked()
    {
        Debug.Log("Button clicked");

        // Tell GameManager to prepare new run
        GameManager.Instance.StartNewRun();

        // Load neighborhood scene
        SceneController.Instance.LoadScene("SampleScene", includeUI: true);
    }

    public void DrawStash()
    { 
        // Draw all the appropriate sprites based on what is in the stash
    }

    public void DrawInventory()
    { 
        // Draw all the appropriate sprites based on what is in the inventory
    }
}

