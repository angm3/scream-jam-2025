using UnityEngine;
using TMPro;

public class GeneralInfo : MonoBehaviour
{
    public StateMachine<GeneralInfo> stateMachine;

    public TMP_Text general_info_text;
    
    void Start()
    {
        
        general_info_text = GameObject.Find("GeneralInfo").GetComponent<TMP_Text>();
        if (general_info_text == null)
        {
            Debug.LogError("GeneralInfo: Could not find GeneralInfo text component");
        }
        else
        {
            Debug.Log("GeneralInfo: Found GeneralInfo text component");
        }
        
        
        stateMachine = new StateMachine<GeneralInfo>();
        // init state is idle
        stateMachine.ChangeState(new GeneralInfoIdleState(this, stateMachine));
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void SetInfo(string text)
    {
        Debug.Log(text);
        general_info_text.text = text;
        stateMachine.ChangeState(new GeneralInfoCooldownState(this, stateMachine));
    }
    
    public void ClearInfo()
    {
        general_info_text.text = "";
    }

}


public class GeneralInfoIdleState : State<GeneralInfo>
{
    public GeneralInfoIdleState(GeneralInfo owner, StateMachine<GeneralInfo> sm) : base(owner, sm) { }

    public override void Enter()
    {
        Debug.Log("info entered idle state");
        owner.ClearInfo();
    }

    public override void Update()
    {
        Debug.Log("info idle state......");
    }
}


public class GeneralInfoCooldownState : State<GeneralInfo>
{
    public GeneralInfoCooldownState(GeneralInfo owner, StateMachine<GeneralInfo> sm) : base(owner, sm) { }
    
    public float cooldown = 1f;
    public float timer;

    public override void Enter()
    {
        Debug.Log("cooldown info state");
        timer = 0f;
    }

    public override void Update()
    {
        Debug.Log("coooooooling");
        timer += Time.deltaTime;
        if (timer >= cooldown)
        {
            owner.stateMachine.ChangeState(new GeneralInfoIdleState(owner, stateMachine));
        }

    }
}