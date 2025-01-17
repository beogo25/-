using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidHitable : MonsterHitablePart
{
    protected DruidAction[] druidAction = new DruidAction[2];
    public override float Hp
    {
        get { return currentHp; }
        set
        {
            currentHp = value;

            if (currentHp <= 0)             // 체력이 0이하가 된다면
            {
                if (isDestructionPart)      // 부위파괴가 가능하다면 
                {
                    isDestructionPart = false;
                    damageMultiplier = partDestructionDamageMultiplier;     // 데미지 배율을 부위파괴시 배율로
                    skinRenderer.gameObject.SetActive(false);
                }

                //Debug.Log(gameObject.name + ", 남은체력 : " + currentHp);
                if (!((DruidStatus)monster).state.HasFlag(MONSTER_STATE.Stagger) && 
                    !((DruidStatus)monster).state.HasFlag(MONSTER_STATE.Dead))    // 경직 및 사망상태가 아니면
                {
                    ((Druid_InBattle)druidAction[1]).StartStaggerState();                      // 경직일으키기
                }

                currentHp = maxhp * 1.2f;   // 체력이 0이하 됐을시 최대체력의 20%를 늘려 체력부여
            }
        }
    }


    private void Start()
    {
        player = FindObjectOfType<PlayerStatus>();
        monster = transform.GetComponentInParent<DruidStatus>();
        druidAction[0] = transform.GetComponentInParent<Druid_SerchingTarget>();
        druidAction[1] = transform.GetComponentInParent<Druid_InBattle>();
        Hp = maxhp;

        //Debug.Log("monster name : " + monster.name + ", monsterAction : " + monsterAction.name);
    }

    public override void Hit(float damage)
    {
        Hp -= damage * damageMultiplier;
        monster.Hp -= damage * damageMultiplier;
        //((DruidStatus)monster).PlayHitSound(Random.Range(0,2));
        if(damage > 100)
            ((DruidStatus)monster).PlayHitSound(1);
        else
            ((DruidStatus)monster).PlayHitSound(0);

        //Debug.Log(gameObject.name + "의 현재 체력 : " + currentHp+ ", 전체체력 : " + monster.Hp +", 데미지 : "+ damage * damageMultiplier);
    }

    // 체력 다 달았을시 경직 일어나는 함수 만들기, 부위별 경직모션 소/중/대/특대, 체력정하고, 데미지 배율
    // 플레이어 공격은 데미지가 고정값인가? -> 파티클에 스크립트를 넣는방식을 할건지? 

    // 경직함수, 한번의 공격으로 여러 콜라이더가 충돌했을시 데미지 여러번 들어가는거 방지하기
}
