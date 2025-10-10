using UnityEngine;

public struct PlayerDamageEvent
{
    public int playerDamage;
    public PlayerDamageEvent(int amount)
    {
        playerDamage = amount;
    }
}