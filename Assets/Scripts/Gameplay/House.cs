using UnityEngine;

public class House : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject lights = transform.Find("Lights").gameObject;

        // for each light in lights, have a 50% chance of being active
        foreach (Transform light in lights.transform)
        {
            if (Random.value > 0.75f)
            {
                light.gameObject.SetActive(true);
            }
            else
            {
                light.gameObject.SetActive(false);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
