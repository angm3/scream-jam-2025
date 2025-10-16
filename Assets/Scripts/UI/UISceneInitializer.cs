using UnityEngine;
using UnityEngine.UI;

public class UISceneInitializer : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject infoPanel;
    public GameObject deathPanel;
    public GameObject victoryPanel;

    [Header("Pause Menu Buttons")]
    public Button pauseResumeButton;
    public Button pauseGameInfoButton;
    public Button pauseExitButton;

    [Header("Info Panel Buttons")]
    public Button infoContinueButton;

    [Header("Victory Screen Buttons")]
    public Button victoryExitButton;
    public Button victoryKeepPlayingButton;

    [Header("Death Screen Buttons")]
    public Button deathBackToGarageButton;
    public Button deathExitButton;

    void Start()
    {
        if (UIManager.Instance != null)
        { 
            UIManager.Instance.RegisterUI(this);
        }
    }
}
