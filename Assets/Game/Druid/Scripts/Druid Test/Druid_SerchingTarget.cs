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
        if (behaviorState != MONSTER_BEHAVIOR_STATE.SerchingTarget) return;     // SerchingTarget ���°� �ƴҰ�� update ����X
        Debug.Log("��ã����");
        if (Input.GetKeyDown(KeyCode.P))
        {
            ChangeState(MONSTER_BEHAVIOR_STATE.InBattle);
        }
    }
    public override void Init()
    {
        Debug.Log("��ã���� Init");
    }

}
