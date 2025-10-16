using UnityEngine;

public class Candy : Collectible
{
    // get reference to the particle system
    public ParticleSystem candyParticles;
    public GameObject candyModel;
    public Light pointLight;
    public Candy()
    {
        type = "candy";
    }
    
    public void Awake()
    {
        candyParticles = GetComponentInChildren<ParticleSystem>();
        // get reference to the model
        candyModel = transform.Find("Model").gameObject;
        pointLight = GetComponentInChildren<Light>();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
           //Debug.Log("Candy trigger");
            GeneralInfo.Instance.SetInfo("Collected Candy!");
            stateMachine.ChangeState(new CollectibleCollectedState(this, stateMachine));
            Collect();
        }
    }


    public void Collect()
    {
        candyModel.SetActive(false);
        AudioManager.Instance.PlaySFX("candy_collect", 1, 0.22f);
        pointLight.enabled = false;
        candyParticles.Play();
        Destroy(this.gameObject, 0.5f);
    }
}
 