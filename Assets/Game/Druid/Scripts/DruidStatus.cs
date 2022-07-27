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
            if (value < currentHp)  // 체력이 닳은 경우에만 실행되도록 
            {
                if (monsterAction.behaviorState != MONSTER_BEHAVIOR_STATE.InBattle)    // 플레이어가 선제공격시 전투모드가 아니면 전투모드
                {
                    Debug.Log("체력이 닳았다면 타겟잡고 전투모드로");
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
                // 죽는 모션

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
