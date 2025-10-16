
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public float driftTimer = 0f;
    private float driftFixTime = 1.0f;
    private bool inDrift = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    //void LateUpdate()
    void FixedUpdate()
    {
        Vector3 player_velocity = GameManager.Instance.GetPlayer().GetComponent<Rigidbody>().linearVelocity;

        if (player_velocity.sqrMagnitude < 0.01f)
            player_velocity = GameManager.Instance.GetPlayer().transform.forward;

        Vector3 dir = player_velocity.normalized;

        Vector3 desiredPos = Vector3.zero;

        if(GameManager.Instance.GetPlayer().GetComponent<BikerTheyThemController>().stateMachine.CurrentState is DriftingState || inDrift) {

            desiredPos = GameManager.Instance.GetPlayer().transform.position - GameManager.Instance.GetPlayer().transform.forward * 6f + Vector3.up * 2.0f;

            if(!inDrift) {
                inDrift = true;
                driftTimer = 0;
            }
            else {
                driftTimer += Time.deltaTime;
                //Debug.Log("Camera Drift Timer: " + driftTimer);
                if(driftTimer > driftFixTime) {
                    inDrift = false;
                    driftTimer = 0f;
                }
            }
            //desiredPos = transform.position;
        }
        else {
            //Debug.Log("Camera Velocity Method");
            desiredPos = GameManager.Instance.GetPlayer().transform.position - dir * 4.5f + Vector3.up * 3.0f;
        }
        
        // smooth camera movement
        Vector3 InterpDesiredPos = Vector3.Lerp(transform.position, desiredPos, 0.1f);
        
        gameObject.transform.position = InterpDesiredPos;
        transform.LookAt(GameManager.Instance.GetPlayer().transform.position + Vector3.up * 1.5f);        
    }
}
