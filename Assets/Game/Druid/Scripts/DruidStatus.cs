using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidStatus : MonsterStatus
{
    [HideInInspector] public MONSTER_BEHAVIOR_STATE behaviorState = MONSTER_BEHAVIOR_STATE.SerchingTarget;
    [HideInInspector] public MONSTER_STATE state = MONSTER_STATE.Idle;
    [HideInInspector] public Collider target;
    private MonsterAction monsterAction;

    public override float Hp
    {
        get { return currentHp; }
        set
        {
            if (value < currentHp)  // ü���� ���� ��쿡�� ����ǵ��� 
            {
                if (monsterAction.behaviorState != MONSTER_BEHAVIOR_STATE.InBattle)    // �÷��̾ �������ݽ� ������尡 �ƴϸ� �������
                {
                    Debug.Log("ü���� ��Ҵٸ� Ÿ����� ��������");
                    Collider[] targets = Physics.OverlapSphere(transform.position, 30f, 1 << LayerMask.NameToLayer("Player"));
                    for (int i = 0; i < targets.Length; i++)
                    {
                        monsterAction.target = targets[i];
                        monsterAction.ChangeState(MONSTER_BEHAVIOR_STATE.InBattle);
                    }
                }
            }

            currentHp = value;
            

            if (currentHp <= 0 && !monsterAction.state.HasFlag(MONSTER_STATE.Dead))     // ü���� 0���ϰ� �ǰ� �������°� �ƴϸ�
            {
                //monsterAction.Dead();
            }
        }
    }
    private void Awake()
    {
        monsterAction = GetComponent<MonsterAction>();
        //behaviorState = MONSTER_BEHAVIOR_STATE.SerchingTarget;
        //state = MONSTER_STATE.Idle;

        maxHp = 20000;
        atk = 10;
        currentHp = maxHp;
    }
    public void Start()
    {
        
        
    }
}
