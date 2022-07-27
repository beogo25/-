using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidStatus : MonsterStatus
{

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
            

            if (currentHp <= 0)
            {
                currentHp = 0;
                // �״� ���

            }
        }
    }
    private void Awake()
    {
        monsterAction = GetComponent<MonsterAction>();

        maxHp = 20000;
        atk = 10;
        currentHp = maxHp;
    }
    public void Start()
    {
        
        
    }
}
