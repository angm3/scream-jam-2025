using UnityEngine;

public struct WeaponTriggerEvent
{
    public string weaponType;
    public WeaponTriggerEvent(string type)
    {
        weaponType = type;
    }
}