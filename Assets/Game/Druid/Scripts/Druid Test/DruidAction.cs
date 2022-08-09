using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MONSTER_BEHAVIOR_STATE
{
    SerchingTarget,
    InBattle,
}

public enum MONSTER_STATE
{
    Idle     = 0,           // 0
    Walk     = 1 << 0,      // 1
    Rotation = 1 << 1,      // 2
    Attack   = 1 << 2,      // 4
    Stagger  = 1 << 3,      // 8
    Dead     = 1 << 4,      // 16

    All = Walk | Rotation | Attack,
}

public abstract class DruidAction : MonoBehaviour
{
    
    protected DruidAction[] druidAction = new DruidAction[2];
    public DruidStatus druidStatus;
    public Animator animator;


    protected void Awake()
    {
        druidAction[0] = GetComponent<Druid_SerchingTarget>();
        druidAction[1] = GetComponent<Druid_InBattle>();

        druidStatus = GetComponent<DruidStatus>();
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        Debug.Log(druidStatus.behaviorState + ", " + animator);
    }
    public abstract void Init();
    public void ChangeState(MONSTER_BEHAVIOR_STATE newState)
    {
        Debug.Log(newState + "로 상태전환시작, " + (int)newState);
        this.enabled = false;

        druidStatus.behaviorState = newState;
        druidAction[(int)newState].enabled = true;
        druidAction[(int)newState].Init();
        Debug.Log(newState + "로 상태전환");
    }


}
