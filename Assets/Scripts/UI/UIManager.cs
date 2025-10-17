using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // script to handle showing and hiding specific ui panels in the UI scene
    public static UIManager Instance { get; private set; }

    //public static DeathScreenPanel deathScreenPanel;
    private GameObject pauseMenu;
    private GameObject infoPanel;
    private GameObject deathPanel;
    private GameObject victoryPanel;

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

    public bool playerClicked = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
        Instance = this;
        //deathScreenPanel = GetComponent<DeathScreenPanel>();
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    void Start()
    {
        //HideAllMenus();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }
    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HidePauseMenu()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu.activeSelf)
            HidePauseMenu();
        else
            ShowPauseMenu();
    }

    public void ShowInfoPanel()
    {
        infoPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HideInfoPanel()
    {
        infoPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ShowVictoryScreen()
    {
        if (playerClicked)
        { 
            victoryPanel.SetActive(true);
        }
    }

    public void HideVictoryScreen()
    {
        victoryPanel.SetActive(false);
    }

    public void ShowDeathPanel()
    { 
        deathPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void HideDeathPanel()
    { 
        deathPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void HideAllMenus()
    {
        Debug.LogWarning("Hiding all Menus in UI Manager");
        pauseMenu.SetActive(false);
        infoPanel.SetActive(false);
        victoryPanel.SetActive(false);
        deathPanel.SetActive(false);
    }

    void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }

    public void RegisterUI(UISceneInitializer uiInit)
    { 
        pauseMenu = uiInit.pauseMenu;
        infoPanel = uiInit.infoPanel;
        deathPanel = uiInit.deathPanel;
        victoryPanel = uiInit.victoryPanel;

        pauseResumeButton = uiInit.pauseResumeButton;
        pauseGameInfoButton = uiInit.pauseGameInfoButton;
        pauseExitButton = uiInit.pauseExitButton;
        infoContinueButton = uiInit.infoContinueButton;
        victoryKeepPlayingButton = uiInit.victoryKeepPlayingButton;
        victoryExitButton = uiInit.victoryExitButton;
        deathBackToGarageButton = uiInit.deathBackToGarageButton;
        deathExitButton = uiInit.deathExitButton;

        // TODO make more specific method calls for these
        pauseResumeButton.onClick.AddListener(HidePauseMenu);
        pauseGameInfoButton.onClick.AddListener(ShowInfoPanel);
        pauseExitButton.onClick.AddListener(QuitGame);
        victoryExitButton.onClick.AddListener(QuitGame);
        infoContinueButton.onClick.AddListener(HideInfoPanel);
        deathBackToGarageButton.onClick.AddListener(OnDeathBackToGarage);
        deathExitButton.onClick.AddListener(QuitGame);
        victoryKeepPlayingButton.onClick.AddListener(HideVictoryScreen);

        HideAllMenus();
        //ShowInfoPanel();
    }

    public void OnDeathBackToGarage()
    {
        Time.timeScale = 1f;
        HideDeathPanel();
        SceneController.Instance.LoadScene("Garage");
    }
    
}
