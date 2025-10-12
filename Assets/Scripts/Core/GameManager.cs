using UnityEngine;

public class GameManager : MonoBehaviour
{

    private GameObject player_ref;
    //public GameObject camera_ref;
    public static GameManager Instance { get; private set; }

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
        }
    }

    public void HandleSuccessfulExtraction ()
    {
        Debug.Log("Extraction Successful");
        // TODO: Transfer inventory items to "stash"
        SceneController.Instance.LoadScene("Garage", includeUI: true);
    }

    // Called when starting a new run from garage
    public void StartNewRun()
    {
        // For now, just reset inventory candy to 0
        // Later: transfer candy from stash to inventory based on UI selection

        Debug.Log("New run started!");
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
        // ...
    }

    // Called by player when scene loads
    public void RegisterPlayer(GameObject player)
    {
        player_ref = player;
    }

    public GameObject GetPlayer() => player_ref;
}