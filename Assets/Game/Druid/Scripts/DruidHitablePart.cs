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

    //        if (currentHp <= 0)             // ü���� 0���ϰ� �ȴٸ�
    //        {
    //            if (isDestructionPart)      // �����ı��� �����ϴٸ� 
    //            {
    //                isDestructionPart = false;
    //                damageMultiplier = partDestructionDamageMultiplier;     // ������ ������ �����ı��� ������
    //                skinRenderer.gameObject.SetActive(false);
    //            }

    //            if (!monsterAction.state.HasFlag(MONSTER_STATE.Stagger))    // �������°� �ƴ϶��
    //            {
    //                Debug.Log(gameObject.name + "����, ����");
    //                monsterAction.state |= MONSTER_STATE.Stagger;
    //                monsterAction.StartStaggerState();                      // ��������Ű��
    //            }

    //            currentHp = maxhp * 1.2f;   // ü���� 0���� ������ �ִ�ü���� 20%�� �÷� ü�ºο�
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
        Debug.Log(gameObject.name + "�� ���� ü�� : " + currentHp + ", ��üü�� : " + monster.Hp + ", ������ : " + damage * damageMultiplier);
    }

    // ü�� �� �޾����� ���� �Ͼ�� �Լ� �����, ������ ������� ��/��/��/Ư��, ü�����ϰ�, ������ ����
    // �÷��̾� ������ �������� �������ΰ�? -> ��ƼŬ�� ��ũ��Ʈ�� �ִ¹���� �Ұ���? 

    // �����Լ�, �ѹ��� �������� ���� �ݶ��̴��� �浹������ ������ ������ ���°� �����ϱ�
}
