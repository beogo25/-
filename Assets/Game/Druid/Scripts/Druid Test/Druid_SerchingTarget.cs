using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Druid_SerchingTarget : DruidAction
{
    void Start()
    {
    }

    void Update()
    {
        if (behaviorState != MONSTER_BEHAVIOR_STATE.SerchingTarget) return;     // SerchingTarget 상태가 아닐경우 update 실행X
        Debug.Log("적찾는중");
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeState(MONSTER_BEHAVIOR_STATE.InBattle);
        }
    }
    public override void Init()
    {
        Debug.Log("적찾는중 Init");
    }

}
