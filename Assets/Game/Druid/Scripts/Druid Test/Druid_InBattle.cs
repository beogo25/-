using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Druid_InBattle : DruidAction
{
    void Start()
    {
    }

    void Update()
    {
        if (druidStatus.behaviorState != MONSTER_BEHAVIOR_STATE.InBattle) return;     // InBattle 상태가 아닐경우 update 실행X
        Debug.Log("전투중");
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeState(MONSTER_BEHAVIOR_STATE.SerchingTarget);
        }
    }
    public override void Init()
    {
        Debug.Log("전투중 Init");
    }

}
