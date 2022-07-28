using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidAttackablePart : MonsterAttackablePart
{
    [SerializeField] private AttackType attackType;
    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        monster = transform.GetComponentInParent<DruidStatus>();
    }

    public void Attack(bool isAttackStart)
    {
        Debug.Log("Attack 실행 : " + isAttackStart);
        if (isAttackStart)
        {
            myCollider.enabled = true;
            isAttackAble = true;
        }
        else
        {
            myCollider.enabled = false;
            isAttackAble = false;
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerStatus>() != null && isAttackAble)
        {
            //Debug.Log(" 데미지 : " + monster.Atk * damageMultiplier);
            isAttackAble = false;
            other.transform.GetComponent<PlayerStatus>().PlayerHit(monster.Atk * damageMultiplier, 20, monster.transform.position);
            myCollider.enabled = false;
        }
    }
}
