using UnityEngine;

public class Monster : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggering damage.");
        EventBus.Publish(new PlayerDamageEvent(7));
    }
}
