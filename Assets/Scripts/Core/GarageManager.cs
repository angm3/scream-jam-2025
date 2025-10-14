using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GarageManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startRunButton;
    [SerializeField] private TextMeshProUGUI counts;


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
}

