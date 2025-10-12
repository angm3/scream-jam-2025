
using UnityEngine;

public class Slingshot : Weapon
{

    public GameObject projectilePrefab;
    
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
        
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        Vector3 relativeMousePosition = Vector3.zero;
        if (playerPlane.Raycast(ray, out distance))
        {
            relativeMousePosition = ray.GetPoint(distance);
        }
        Debug.Log("relativeMousePosition = " + (relativeMousePosition - GameManager.Instance.GetPlayer().transform.position));

        //EventBus.Publish(new PlayerBumpEvent((GameManager.Instance.GetPlayer().transform.position - relativeMousePosition), 40f));

        /*
        Vector3 bump_dir = Vector3.zero;
        if (relativeMousePosition.z > 0)
        {
            bump_dir = new Vector3(0, 0, -1f);
        }
        else if (relativeMousePosition.z < 0)
        {
            bump_dir = new Vector3(0, 0, 1f);
        }
        EventBus.Publish(new PlayerBumpEvent(bump_dir, 40f));
        */

        Vector3 baes_projectile_dir = -(GameManager.Instance.GetPlayer().transform.position - relativeMousePosition).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<Rigidbody>().linearVelocity = baes_projectile_dir * 30f;
    }
}
