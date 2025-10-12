using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingCanvas;

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
        LoadScene("MainMenu");
    }

    // Simple single scene load
    public void LoadScene(string sceneName, bool includeUI = false)
    {
        SceneManager.LoadScene(sceneName);
        //StartCoroutine(LoadSceneAsync(sceneName, includeUI));
    }

    private IEnumerator LoadSceneAsync(string targetScene, bool loadUI)
    {
        // Show loading
        if (loadingCanvas != null)
            loadingCanvas.SetActive(true);

        // Just load the scene normally (replaces current scene)
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(targetScene);

        while (!loadOp.isDone)
        {
            yield return null;
        }

        // If UI needed, load it additively
        if (loadUI)
        {
            if (!SceneManager.GetSceneByName("UI").isLoaded)
            {
                yield return SceneManager.LoadSceneAsync("UI", LoadSceneMode.Additive);
            }
        }
        else
        {
            // Unload UI if not needed
            if (SceneManager.GetSceneByName("UI").isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync("UI");
            }
        }

        yield return new WaitForSeconds(0.3f);

        if (loadingCanvas != null)
            loadingCanvas.SetActive(false);

        EventBus.Publish(new SceneChangeEvent(targetScene));
    }
}