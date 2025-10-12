
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
        //Debug.Log("mouseDown = " + Input.mousePosition.x + " " + Input.mousePosition.y);
        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
        //Vector3 dir = (worldPos - transform.position).normalized;
        //Debug.Log("worldPos = " + worldPos + " dir = " + dir);
        
        
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        Vector3 relativeMousePosition = Vector3.zero;
        if (playerPlane.Raycast(ray, out distance))
        {
            relativeMousePosition = ray.GetPoint(distance);
        }
        Debug.Log("relativeMousePosition = " + (relativeMousePosition - GameManager.Instance.GetPlayer().transform.position));
        
        EventBus.Publish(new PlayerBumpEvent((GameManager.Instance.GetPlayer().transform.position - relativeMousePosition), 40f));
    }
}
