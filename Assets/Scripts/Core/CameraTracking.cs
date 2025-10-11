using UnityEngine;

public class CameraTracking : MonoBehaviour
{

    private Vector3 delta_pos = new Vector3(-7, 2.8f, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 biker_pos = GameManager.Instance.player_ref.transform.position;
        Vector3 biker_pos = GameManager.Instance.GetPlayerTransform().position;
        //Vector3 camera_pos = biker_pos + delta_pos;
        Vector3 camera_pos = biker_pos + delta_pos;
        //gameObject.transform.position = camera_pos;
        
    }
}
