using UnityEngine;

public class GameManager : MonoBehaviour
{

    //public GameObject player_ref;
    //public GameObject camera_ref;
    public static GameManager Instance { get; private set; }

    private Transform playerTransform;

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
    public void RegisterPlayer(Transform player)
    {
        playerTransform = player;
    }

    public Transform GetPlayerTransform() => playerTransform;
}