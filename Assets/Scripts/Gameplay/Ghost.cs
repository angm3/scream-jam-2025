using UnityEngine;

public class Ghost : Monster
{
    
    public override void Chase()
    {
        //Debug.Log("Ghost is chasing the player!");
        // Implement ghost-specific chasing behavior here
        
        gameObject.transform.position = Vector3.MoveTowards(
            gameObject.transform.position, 
            GameManager.Instance.player_ref.transform.position, 
            0.01f);
    }

}
