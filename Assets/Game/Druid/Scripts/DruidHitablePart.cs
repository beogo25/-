using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidHitablePart : MonsterHitablePart
{

    //public override float Hp
    //{
    //    get { return currentHp; }
    //    set
    //    {
    //        currentHp = value;

    //        if (currentHp <= 0)             // 체력이 0이하가 된다면
    //        {
    //            if (isDestructionPart)      // 부위파괴가 가능하다면 
    //            {
    //                isDestructionPart = false;
    //                damageMultiplier = partDestructionDamageMultiplier;     // 데미지 배율을 부위파괴시 배율로
    //                skinRenderer.gameObject.SetActive(false);
    //            }

    //            if (!monsterAction.state.HasFlag(MONSTER_STATE.Stagger))    // 경직상태가 아니라면
    //            {
    //                Debug.Log(gameObject.name + "부위, 경직");
    //                monsterAction.state |= MONSTER_STATE.Stagger;
    //                monsterAction.StartStaggerState();                      // 경직일으키기
    //            }

    //            currentHp = maxhp * 1.2f;   // 체력이 0이하 됐을시 최대체력의 20%를 늘려 체력부여
    //        }
    //    }
    //}


    //private void Start()
    //{
    //    player = FindObjectOfType<PlayerStatus>();
    //    monster = transform.GetComponentInParent<DruidStatus>();
    //    monsterAction = transform.GetComponentInParent<MonsterAction>();
    //    Hp = maxhp;

    //    //Debug.Log("monster name : " + monster.name + ", monsterAction : " + monsterAction.name);
    //}

    public override void Hit(float damage)
    {
        Hp -= damage * damageMultiplier;
        monster.Hp -= damage * damageMultiplier;
        Debug.Log(gameObject.name + "의 현재 체력 : " + currentHp + ", 전체체력 : " + monster.Hp + ", 데미지 : " + damage * damageMultiplier);
    }

    // 체력 다 달았을시 경직 일어나는 함수 만들기, 부위별 경직모션 소/중/대/특대, 체력정하고, 데미지 배율
    // 플레이어 공격은 데미지가 고정값인가? -> 파티클에 스크립트를 넣는방식을 할건지? 

    // 경직함수, 한번의 공격으로 여러 콜라이더가 충돌했을시 데미지 여러번 들어가는거 방지하기
}
