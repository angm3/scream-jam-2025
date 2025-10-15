using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        // Wire up button listeners
        if (startButton != null)
            startButton.onClick.AddListener(OnStartGame);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettings);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);
    }

    private void OnStartGame()
    {
        GameManager.Instance.StartNewRun();
        SceneController.Instance.LoadScene("SampleScene", includeUI: true);
    }

    private void OnSettings()
    {
        // TODO: Open settings panel
        Debug.Log("Settings Clicked");
    }

    private void OnQuit()
    {
        Debug.Log("Quit Clicked");
    }
}