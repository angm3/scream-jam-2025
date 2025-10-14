using System.Collections;
using UnityEngine;

public class ExtractionPoint : MonoBehaviour
{
    [SerializeField]
    public int extractionTime = 10;

    private Coroutine extractionCoroutine;
    private bool isExtracting = false;

    private void StartExtraction()
    {
        if (isExtracting) return; // Already extracting

        isExtracting = true;
        Debug.Log("Extraction started - stay in zone!");
        extractionCoroutine = StartCoroutine(ExtractionTimer());
    }

    private void CompleteExtraction()
    {
        isExtracting = false;
        extractionCoroutine = null;

        Debug.Log("Extraction complete!");

        // Hand off to ExtractionManager
        GameManager.Instance?.HandleSuccessfulExtraction();
    }

    private void CancelExtraction()
    {
        if (!isExtracting) return;

        isExtracting = false;
        Debug.Log("Extraction cancelled - left zone!");

        if (extractionCoroutine != null)
        {
            StopCoroutine(extractionCoroutine);
            extractionCoroutine = null;
        }
    }

    private IEnumerator ExtractionTimer()
    {
        float elapsed = 0f;

        while (elapsed < extractionTime)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / extractionTime;

            // Optional: Show progress
            Debug.Log($"Extracting... {progress * 100:F0}%");
            GameManager.Instance.general_info.SetInfo($"Extracting... {progress * 100:F0}%");

            yield return null;
        }

        // Extraction complete!
        CompleteExtraction();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Extraction started");
        //StartExtractTimer(extractTimer);

        if (other.CompareTag("Player"))
        {
            StartExtraction();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Extraction stopped");
        if (other.CompareTag("Player"))
        {
            CancelExtraction();
        }
    }
}
