

using UnityEngine;

public class PlayerBumpEvent
{
    
    public Vector3 direction;
    public float mag;

    public PlayerBumpEvent(Vector3 dir, float amount)
    {
        direction = dir;
        mag = amount;
    }
}
