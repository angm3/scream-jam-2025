using System.Diagnostics;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    
    void OnEnable()
    {
        EventBus.Subscribe<WeaponTriggerEvent>(TriggerWeapon);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<WeaponTriggerEvent>(TriggerWeapon);
    }
    
    public abstract void TriggerWeapon(WeaponTriggerEvent e);
}
