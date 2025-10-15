using UnityEngine;
using System;

public class ClickableObject : MonoBehaviour
{
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
                // Debug.Log("hit " + hit.collider.gameObject.name);
                if (hit.collider.TryGetComponent(out ClickableObject clickableObject))
                {
                    Debug.Log($"Clicked clickableObject: {clickableObject.name}");
                    OnClicked?.Invoke(clickableObject.name);
                }
            }
            // else
            // {
            //     Debug.Log("No hit");
            // }
        }
    }
}