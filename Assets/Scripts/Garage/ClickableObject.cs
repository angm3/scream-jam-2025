using UnityEngine;
using System;

public class ClickableObject : MonoBehaviour
{
    //     // Event triggered when any ClickableObject is clicked
    //     public static event Action<string> OnClicked;
    // 
    //      not doing anything
    //     private void OnMouseDown()
    //     {
    //         Debug.Log($"[ClickableObject] {name} clicked");
    //         OnClicked?.Invoke(name);
    //     }
    public static event System.Action<string> OnClicked;

    // Collider must exist on this GameObject

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            // Debug.Log("Click");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // Debug.Log("Input.mousePosition" + Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("hit " + hit.collider.gameObject.name);
                var clickable = hit.collider.GetComponent<ClickableObject>();
                if (clickable != null)
                {
                    Debug.Log($"Clicked object: {clickable.name}");
                    OnClicked?.Invoke(clickable.name);
                }
            } else
            {
                Debug.Log("No hit");
            }
        }
    }
}