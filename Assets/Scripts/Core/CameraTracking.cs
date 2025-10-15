
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    public float driftTimer = 0f;
    private float driftFixTime = 1.0f;
    private bool inDrift = false;

    // camera distance params
    public float normalDistance = 5f;
    public float normalHeight = 3f;
    public float driftDistance = 8f;
    public float driftHeight = 3.5f;

    public float normalFOV = 35f;
    public float speedFOV = 60f;
    public float fovLerpSpeed = 2f;

    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.fieldOfView = normalFOV;
        }
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

            //desiredPos = GameManager.Instance.GetPlayer().transform.position - GameManager.Instance.GetPlayer().transform.forward * 6f + Vector3.up * 2.0f;
            desiredPos = GameManager.Instance.GetPlayer().transform.position - GameManager.Instance.GetPlayer().transform.forward * driftDistance + Vector3.up * driftHeight;

            if (!inDrift) {
                inDrift = true;
                driftTimer = 0;
            }
            else {
                driftTimer += Time.deltaTime;
                Debug.Log("Camera Drift Timer: " + driftTimer);
                if(driftTimer > driftFixTime) {
                    inDrift = false;
                    driftTimer = 0f;
                }
            }
            //desiredPos = transform.position;
        }
        else {
            Debug.Log("Camera Velocity Method");
            //desiredPos = GameManager.Instance.GetPlayer().transform.position - dir * 3f + Vector3.up * 2.0f;
            desiredPos = GameManager.Instance.GetPlayer().transform.position - dir * normalDistance + Vector3.up * normalHeight;
        }
        
        // smooth camera movement
        Vector3 InterpDesiredPos = Vector3.Lerp(transform.position, desiredPos, 0.1f);
        gameObject.transform.position = InterpDesiredPos;
        //transform.LookAt(GameManager.Instance.GetPlayer().transform.position + Vector3.up * 1.5f);
        transform.LookAt(GameManager.Instance.GetPlayer().transform.position + Vector3.up * 0.7f);

        float speed = player_velocity.magnitude;
        bool atMaxSpeed = GameManager.Instance.GetPlayer().GetComponent<BikerTheyThemController>().checkMaxSpeedEffectThreshold();
        float targetFOV = atMaxSpeed ? speedFOV : normalFOV;

        if (cam != null)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
        }
    }
}
