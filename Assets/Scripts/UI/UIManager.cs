using UnityEngine;

public class UIManager : MonoBehaviour
{
    // script to handle showing and hiding specific ui panels in the UI scene
    public static UIManager Instance { get; private set; }

    public static DeathScreenPanel deathScreenPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
        Instance = this;
        deathScreenPanel = GetComponent<DeathScreenPanel>();
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    public void SetDeathScreenPanelActive()
    {
        deathScreenPanel.gameObject.SetActive(true);
    }

    public void SetDeathScreenPanelInactive()
    {
        deathScreenPanel.gameObject.SetActive(false);
    }
    

}
