
using UnityEngine;

public class CameraTracking : MonoBehaviour
{

    private Vector3 delta_pos = new Vector3(-7, 2.8f, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 player_velocity = GameManager.Instance.GetPlayer().GetComponent<Rigidbody>().linearVelocity;

        if (player_velocity.sqrMagnitude < 0.01f)
            player_velocity = GameManager.Instance.GetPlayer().transform.forward;

        Vector3 dir = player_velocity.normalized;

        Vector3 desiredPos = GameManager.Instance.GetPlayer().transform.position - dir * 7f + Vector3.up * 2.8f;

        gameObject.transform.position = desiredPos;
        transform.LookAt(GameManager.Instance.GetPlayer().transform.position + Vector3.up * 1.5f);        
    }
}
