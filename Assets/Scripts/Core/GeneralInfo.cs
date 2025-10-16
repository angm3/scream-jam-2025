using UnityEngine;
using TMPro;

public class GeneralInfo : MonoBehaviour
{
    public StateMachine<GeneralInfo> stateMachine;

    public TMP_Text general_info_text;

    public static GeneralInfo Instance { get; private set; }
    
    public float current_alpha = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
    }

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

    public void GetReferenceToText()
    {
        general_info_text = GameObject.Find("GeneralInfo").GetComponent<TMP_Text>();
    }

    public void SetInfo(string text)
    {
        Debug.Log(text);
        if (general_info_text == null)
        {
            GetReferenceToText();
        }
        general_info_text.text = text;
        stateMachine.ChangeState(new GeneralInfoCooldownState(this, stateMachine));
    }

    public void ClearInfo()
    {
        general_info_text.text = "";
    }
    
    public void SetTransparency(float alpha)
    {
        Color color = general_info_text.color;
        color.a = alpha;
        general_info_text.color = color;
    }

}



public class GeneralInfoSetState : State<GeneralInfo>
{
    public GeneralInfoSetState(GeneralInfo owner, StateMachine<GeneralInfo> sm) : base(owner, sm) { }

    public float fade_in_time;
    public float timer;

    public override void Enter()
    {
        Debug.Log("info entered set state");
        owner.ClearInfo();
        owner.SetTransparency(0f);
        timer = 0f;
    }

    public override void Update()
    {
        Debug.Log("info set state......");
        // fade in text
        float  alpha = Mathf.Clamp01(timer / fade_in_time);
        owner.SetTransparency(alpha);
        timer += Time.deltaTime;
        if (alpha >= 1f)
        {
            owner.stateMachine.ChangeState(new GeneralInfoCooldownState(owner, stateMachine));
            timer = 0f;
        }
    }
}




public class GeneralInfoIdleState : State<GeneralInfo>
{
    public GeneralInfoIdleState(GeneralInfo owner, StateMachine<GeneralInfo> sm) : base(owner, sm) { }

    public override void Enter()
    {
        Debug.Log("info entered idle state");
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
        owner.SetTransparency(1f);
    }

    public override void Update()
    {
        Debug.Log("coooooooling");
        timer += Time.deltaTime;
        owner.SetTransparency(1f - Mathf.Clamp01(timer / cooldown));
        if (timer >= cooldown)
        {
            timer = 0f;
            owner.ClearInfo();
            owner.SetTransparency(0f);
            owner.stateMachine.ChangeState(new GeneralInfoIdleState(owner, stateMachine));
        }

    }
}