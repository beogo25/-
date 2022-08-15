using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidStatus : MonsterStatus, IInteraction
{
    [HideInInspector] public MONSTER_BEHAVIOR_STATE behaviorState = MONSTER_BEHAVIOR_STATE.SerchingTarget;
    [HideInInspector] public MONSTER_STATE state = MONSTER_STATE.Idle;
    [HideInInspector] public Collider target;
    private DruidAction[] druidAction = new DruidAction[2];

    public override float Hp
    {
        get { return currentHp; }
        set
        {
            if (value < currentHp)  // ü���� ���� ��쿡�� ����ǵ��� 
            {
                if (behaviorState != MONSTER_BEHAVIOR_STATE.InBattle)    // �÷��̾ �������ݽ� ������尡 �ƴϸ� �������
                {
                    Debug.Log("ü���� ��Ҵٸ� Ÿ����� ��������");
                    Collider[] targets = Physics.OverlapSphere(transform.position, 30f, 1 << LayerMask.NameToLayer("Character"));
                    for (int i = 0; i < targets.Length; i++)
                    {
                        target = targets[i];
                        druidAction[0].ChangeState(MONSTER_BEHAVIOR_STATE.InBattle);
                    }
                }
            }

            currentHp = value;
            Debug.Log("����ü�� : " + currentHp);
            if (currentHp <= 0 && !state.HasFlag(MONSTER_STATE.Dead))     // ü���� 0���ϰ� �ǰ� �������°� �ƴϸ�
            {
                ((Druid_InBattle)druidAction[1]).Dead();
            }
        }
    }
    private void Awake()
    {
        druidAction[0] = GetComponent<Druid_SerchingTarget>();
        druidAction[1] = GetComponent<Druid_InBattle>();

        maxHp = 1000;
        atk = 10;
        currentHp = maxHp;
    }


    public void Interaction()
    {
        
    }
}
