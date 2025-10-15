using UnityEngine;
using System.Collections;



public class Ghost : Monster<Ghost>
{
    private Healthbar healthbar;
    [SerializeField] private int playerHitDamage = 100;
    [SerializeField] private int projectileHitDamage = 50;
    
    private void Start()
    {
        stateMachine.ChangeState(new GhostIdleState(this, stateMachine));
        playerDamage = 30;
        maxHealth = 100;
        currentHealth = 100;
        healthbar = GetComponentInChildren<Healthbar>();
        Debug.Log(healthbar);

        // Set/Add a Rigidbody if it doesn't exist
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnEnable()
    {
        EventBus.Subscribe<EnemyDamageEvent>(TakeDamage);
       
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<EnemyDamageEvent>(TakeDamage);
       
    }

    public bool checkIfVelocityIsForward(Rigidbody rb)
    {
        return Vector3.Dot(rb.transform.forward, rb.linearVelocity) > 0;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collison detected in ghost");

        if (other.gameObject.CompareTag("Player"))
        {
            if (stateMachine.CurrentState is GhostAttackingState)
            {
                Debug.Log("Player hit by ghost");
                EventBus.Publish(new PlayerDamageEvent(playerDamage));
            }
            else if (stateMachine.CurrentState is GhostChasingState || stateMachine.CurrentState is GhostIdleState)
            {
                Debug.Log("Ghost hit by player");
                if (other.gameObject.GetComponent<BikerTheyThemController>().checkSpeedEffectThreshold())
                {
                    TakeDamage(new EnemyDamageEvent(playerHitDamage));
                    healthbar.UpdateHealthBar(maxHealth, currentHealth);
                }
            }
        }

        if (other.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("Ghost hit by projectile");
            TakeDamage(new EnemyDamageEvent(projectileHitDamage));
            healthbar.UpdateHealthBar(maxHealth, currentHealth);
            
        }

        if (currentHealth <= 0)
        {
            Debug.Log("Ghost has no health, dying");
            stateMachine.ChangeState(new GhostDyingState(this, stateMachine));
        }
    }

    public void TakeDamage(EnemyDamageEvent e)
    {
        Debug.Log("Ghost took damage: " + e.enemyDamage.ToString());
        currentHealth -= e.enemyDamage;
    }

    

    public void SetColorOfLight(Color color)
    {
        Light pointLight = GetComponentInChildren<Light>();
        
        if (pointLight != null)
        {
            pointLight.color = color;
        }
    }
    
}


public class GhostIdleState : MonsterIdleState<Ghost>
{
    public GhostIdleState(Ghost owner, StateMachine<Ghost> sm) : base(owner, sm) { }

    float bobPhase;

    public override void Enter()
    {
        Debug.Log("Entered Ghost Idle");

        owner.SetColorOfLight(new Color(1f, 1f, 0f, 1f));
        bobPhase = Random.Range(0f, 2f * Mathf.PI);
    }

    public override void Update()
    {

        // make the ghost bob up and down
        float bobHeight = 0.5f;
        float bobSpeed = 2f;
        // random bob phase
        owner.transform.position = new Vector3(owner.transform.position.x, owner.transform.position.y + Mathf.Sin(bobPhase +Time.time * bobSpeed) * bobHeight * Time.deltaTime, owner.transform.position.z);


        if ((GameManager.Instance.GetPlayer().transform.position - this.owner.gameObject.transform.position).magnitude < 10f)
        {
            stateMachine.ChangeState(new GhostChasingState(owner, stateMachine));
        }
    }
}


public class GhostChasingState : MonsterChasingState<Ghost>
{
    public GhostChasingState(Ghost owner, StateMachine<Ghost> sm) : base(owner, sm) { }

    public float attack_cooldown = 5f;
    public float attack_cooldown_timer;
    public bool can_attack;
    public Vector3 offset;
    

    public override void Enter()
    {

        Debug.Log("Ghost is chasing");
        owner.SetColorOfLight(new Color(255f / 255f, 154f / 255f, 0f, 255f));
        
        attack_cooldown_timer = 3f;
        can_attack = false;

        // Random sphere offset for ghost
        float randomSphereRadius = 2f;
        offset = Random.onUnitSphere * randomSphereRadius;
        offset.y = (offset.y < 0f ? -offset.y : offset.y / randomSphereRadius);
    }

    public override void Update()
    {

        // Rotate -x towards player
        Vector3 directionToPlayer = (GameManager.Instance.GetPlayer().transform.position - owner.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, Time.deltaTime*2f);
        
        
        if (owner.checkIfVelocityIsForward(GameManager.Instance.GetPlayer().GetComponent<Rigidbody>()))
        {
            Vector3 desiredPose = Vector3.MoveTowards(
                owner.gameObject.transform.position,
                GameManager.Instance.GetPlayer().transform.position + Vector3.up * 0.2f + GameManager.Instance.GetPlayer().GetComponent<Rigidbody>().linearVelocity * 2f + offset,
                Time.deltaTime*6f);

            //owner.gameObject.transform.position = checkForPhaseThroughPlayer(desiredPose) ? owner.gameObject.transform.position : desiredPose;
            owner.gameObject.transform.position = desiredPose;
        }
        else
        {
            Vector3 desiredPose = Vector3.MoveTowards(
                owner.gameObject.transform.position,
                GameManager.Instance.GetPlayer().transform.position + Vector3.up * 0.2f - GameManager.Instance.GetPlayer().transform.forward * 1f + offset,
                Time.deltaTime*4f);

            //owner.gameObject.transform.position = checkForPhaseThroughPlayer(desiredPose) ? owner.gameObject.transform.position : desiredPose;
            owner.gameObject.transform.position = desiredPose;
        }

        if ((GameManager.Instance.GetPlayer().transform.position - owner.transform.position).magnitude < 7f && can_attack)
        {
            stateMachine.ChangeState(new GhostAttackingState(owner, stateMachine));
        }

        else if ((GameManager.Instance.GetPlayer().transform.position - owner.transform.position).magnitude > 15f)
        {
            stateMachine.ChangeState(new GhostIdleState(owner, stateMachine));
        }
    }

    public override void FixedUpdate()
    {
        if (!can_attack)
        {
            attack_cooldown_timer += Time.fixedDeltaTime;
            if (attack_cooldown_timer >= attack_cooldown)
            {
                can_attack = true;
                attack_cooldown_timer = 0f;
            }
        }
    }

    private bool checkForPhaseThroughPlayer(Vector3 desiredPose)
    {
        // Current Ghost Position minus Player Position
        Vector3 currentPoseDelta = owner.gameObject.transform.position - GameManager.Instance.GetPlayer().transform.position;
        
        // Desired Ghost Position minus Player Position
        Vector3 desiredPoseDelta = desiredPose - GameManager.Instance.GetPlayer().transform.position;

        // Return true if new position is on the other side of the player, otherwise return false if not (false is the desired behavior)
        return Vector3.Dot(currentPoseDelta, desiredPoseDelta) < 0;
    }
}



public class GhostAttackingState : MonsterAttackingState<Ghost>
{
    public GhostAttackingState(Ghost owner, StateMachine<Ghost> sm) : base(owner, sm) { }
    public Coroutine dashCoroutine;
    public bool is_projecting;
    public float attack_windup_time;
    public Vector3 direction;
    public Vector3 targetPose;
    public Vector3 targetPoseOffset;
    public Quaternion targetRotation;

    public override void Enter()
    {
        Debug.Log("Ghost is attacking");
        owner.SetColorOfLight(new Color(255f / 255f, 0f / 255f, 0f, 255f));
        is_projecting = true;

        // Windup for an attack
        //attack_windup_time = Random.Range(0.5f, 1.0f);
        attack_windup_time = 1f;

        dashCoroutine = owner.StartCoroutine(DashAtPlayer());
    }

    IEnumerator DashAtPlayer()
    {
        //yield return new WaitForSeconds(attack_windup_time);
        float elapsedTime = 0f;

        while(elapsedTime < attack_windup_time)
        {
            elapsedTime += Time.deltaTime;

            // This block determines where the ghost looks
            targetPose = GameManager.Instance.GetPlayer().transform.position + GameManager.Instance.GetPlayer().GetComponent<Rigidbody>().linearVelocity * (attack_windup_time - elapsedTime) * 0.0f;
            direction = (targetPose - owner.transform.position).normalized;
            targetRotation = Quaternion.LookRotation(direction);
            
            // This affects the rotation and position of the ghost
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, Time.deltaTime*2f);
            owner.transform.position += (GameManager.Instance.GetPlayer().GetComponent<Rigidbody>().linearVelocity * 1.4f + Vector3.up * 1.5f) * Time.deltaTime;

            yield return null;
        }

        // Add a bit of randomness to the exact point the ghost attacks
        float randomSphereRadius = 1.75f;
        targetPoseOffset = Random.onUnitSphere * randomSphereRadius;
        // Force y pose to be positive and between 0 and 1
        targetPoseOffset.y = (targetPoseOffset.y < 0 ? -targetPoseOffset.y : targetPoseOffset.y) / randomSphereRadius;
        direction = (GameManager.Instance.GetPlayer().transform.position + targetPoseOffset - owner.transform.position).normalized;
        targetRotation = Quaternion.LookRotation(direction);

        is_projecting = false;

        float dashSpeed = 20f;
        float dashDuration = 0.4f;
        elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, Time.deltaTime*2f);
            owner.transform.position += direction * dashSpeed * Time.deltaTime;

            // Ensure the ghost doesn't dash too far into the ground
            if(owner.transform.position.y < GameManager.Instance.GetPlayer().transform.position.y - 0.5f)
            {
                Vector3 pos = owner.transform.position;
                pos.y = GameManager.Instance.GetPlayer().transform.position.y - 0.2f;
                owner.transform.position = pos;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Debug.Log("Done dashong" + Time.time);

        if (!(stateMachine.CurrentState is GhostDyingState))
        {
            // Force the ghost to wait/give the player time to get away
            yield return new WaitForSeconds(1f);
            stateMachine.ChangeState(new GhostIdleState(owner, stateMachine));
        }
    }
}



public class GhostDyingState : MonsterDyingState<Ghost>
{
    private float duration = 1f;      // total lifetime of the dying effect
    private float elapsed = 0f;
    private Vector3 startScale;
    private Vector3 targetScale = Vector3.zero;
    private float rotationSpeed = 360f;   // degrees per second
    private float riseHeight = 2f;        // how far upward it rises
    public Vector3 initialPosition;

    public GhostDyingState(Ghost owner, StateMachine<Ghost> sm) : base(owner, sm) { }

    public override void Enter()
    {
        Debug.Log("Ghost is dying");
        elapsed = 0f;
        startScale = owner.transform.localScale;
        initialPosition = owner.transform.position;
    }

    public override void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        // Rise upward in a helical motion (spiral)
        float angle = t * Mathf.PI * 4f; // number of twists (2 full turns)
        float radius = Mathf.Lerp(0.5f, 0f, t); // spiral tightens as it rises
        Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, Mathf.Lerp(0, riseHeight, t), Mathf.Sin(angle) * radius);

        owner.transform.position = initialPosition + offset;

        // Rotate around its own Y axis
        owner.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

        // Shrink over time
        owner.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

        // When done, destroy
        if (t >= 1f)
        {
            Object.Destroy(owner.gameObject);
        }
    }
}