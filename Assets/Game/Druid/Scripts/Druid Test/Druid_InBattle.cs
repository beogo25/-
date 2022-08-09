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
        if (druidStatus.behaviorState != MONSTER_BEHAVIOR_STATE.InBattle) return;     // InBattle ���°� �ƴҰ�� update ����X
        Debug.Log("������");
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeState(MONSTER_BEHAVIOR_STATE.SerchingTarget);
        }
    }
    public override void Init()
    {
        Debug.Log("������ Init");
    }

}
