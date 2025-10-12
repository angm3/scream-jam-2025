using UnityEngine;

public class Ghost : Monster
{

    public float speed = 0.1f;

    public override void Chase()
    {
        //Debug.Log("Ghost is chasing the player!");
        // Implement ghost-specific chasing behavior here
        
        if(checkIfVelocityIsForward(GameManager.Instance.GetPlayer().GetComponent<Rigidbody>())) 
        {
            gameObject.transform.position = Vector3.MoveTowards(
                gameObject.transform.position,
                GameManager.Instance.GetPlayer().transform.position + GameManager.Instance.GetPlayer().GetComponent<Rigidbody>().linearVelocity * 2f,
                0.01f);
        }
        else
        {
            gameObject.transform.position = Vector3.MoveTowards(
                gameObject.transform.position,
                GameManager.Instance.GetPlayer().transform.position,
                0.01f);
        }
            
    }

    public bool checkIfVelocityIsForward(Rigidbody rb)
    {
        return Vector3.Dot(rb.transform.forward, rb.linearVelocity) > 0;
    }
}
