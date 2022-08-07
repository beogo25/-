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
    static public MONSTER_BEHAVIOR_STATE behaviorState = MONSTER_BEHAVIOR_STATE.SerchingTarget;
    protected DruidAction[] druidAction = new DruidAction[2];

    private void Awake()
    {
        druidAction[0] = GetComponent<Druid_SerchingTarget>();
        druidAction[1] = GetComponent<Druid_InBattle>();
    }

    public abstract void Init();
    public void ChangeState(MONSTER_BEHAVIOR_STATE newState)
    {
        Debug.Log(newState + "로 상태전환시작, " + (int)newState);
        this.enabled = false;

        behaviorState = newState;
        druidAction[(int)newState].enabled = true;
        druidAction[(int)newState].Init();
        Debug.Log(newState + "로 상태전환");
    }


}
