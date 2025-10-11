using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingCanvas;

    private const string ROOT = "Root";
    private const string UI = "UI";
    private const string MAIN_MENU = "MainMenu";
    private const string GARAGE = "Garage";
    private const string NEIGHBORHOOD = "Neighborhood";
    private const string TEST = "SampleScene";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (loadingCanvas != null)
            loadingCanvas.SetActive(false);
    }

    private void Start()
    {
        LoadScene(MAIN_MENU);
    }

    public void LoadScene(string sceneName, bool includeUI = false)
    {
        StartCoroutine(LoadSceneAsync(sceneName, includeUI));
    }

    private IEnumerator LoadSceneAsync(string targetScene, bool loadUI)
    {
        // Show loading screen
        if (loadingCanvas != null)
            loadingCanvas.SetActive(true);

        // Unload all scenes except Root
        for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name != ROOT && scene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }

        // Load target scene
        yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetScene));

        // Load UI if needed
        if (loadUI && !SceneManager.GetSceneByName(UI).isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(UI, LoadSceneMode.Additive);
        }

        // Brief pause
        yield return new WaitForSeconds(0.3f);

        // Hide loading screen
        if (loadingCanvas != null)
            loadingCanvas.SetActive(false);

        EventBus.Publish(new SceneChangeEvent(targetScene));
    }
}