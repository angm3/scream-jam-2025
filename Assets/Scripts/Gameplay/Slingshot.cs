using UnityEngine;

public class Slingshot : Weapon
{

    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EventBus.Publish(new WeaponTriggerEvent("Slingshot"));
        }

    }

    public override void TriggerWeapon(WeaponTriggerEvent e)
    {
        Debug.Log("Slingshot fired!");
        // Implement slingshot-specific behavior here
    }
}
