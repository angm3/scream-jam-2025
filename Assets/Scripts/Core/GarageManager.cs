using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GarageManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startRunButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("=== GARAGE MANAGER START ===");
        Debug.Log($"GarageManager on GameObject: {gameObject.name}");
        Debug.Log($"Button assigned: {startRunButton != null}");

        if (startRunButton != null)
        {
            Debug.Log($"Button GameObject: {startRunButton.gameObject.name}");
            Debug.Log($"Button interactable: {startRunButton.interactable}");
            Debug.Log($"Button has Image: {startRunButton.GetComponent<UnityEngine.UI.Image>() != null}");

            startRunButton.onClick.AddListener(OnStartRunClicked);
        }
    }

    public void OnStartRunClicked()
    {
        // Set spawn point for neighborhood

        // Tell GameManager to prepare new run
        GameManager.Instance.StartNewRun();

        // Load neighborhood scene
        SceneController.Instance.LoadScene("Neighborhood", includeUI: true);
    }
}

public class EventSystemDebug : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("=== MOUSE CLICKED ===");

            EventSystem es = EventSystem.current;
            if (es == null)
            {
                Debug.LogError("NO EVENTSYSTEM FOUND!");
                return;
            }

            Debug.Log($"EventSystem exists: {es.gameObject.name}");
            Debug.Log($"EventSystem enabled: {es.enabled}");

            // Check what's under the mouse
            PointerEventData pointerData = new PointerEventData(es);
            pointerData.position = Input.mousePosition;

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            Debug.Log($"Raycasts found: {results.Count}");
            foreach (var result in results)
            {
                Debug.Log($"Hit: {result.gameObject.name}");
            }
        }
    }
}
