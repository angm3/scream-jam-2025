using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject player_ref;
    public GameObject camera_ref;
    public static GameManager Instance { get; private set; }

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
}