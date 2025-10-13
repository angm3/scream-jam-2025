using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Destroy the projectile after 5 seconds to prevent memory leaks
        Destroy(gameObject, 5f);
    }
    
}
