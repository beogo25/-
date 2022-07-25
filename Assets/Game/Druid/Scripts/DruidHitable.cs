using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidHitable : MonsterHitablePart
{


    public override float Hp
    {
        get { return currentHp; }
        set
        {
            currentHp = value;

            if (currentHp <= 0)
            {
                if (isDestructionPart)   // �����ı��� �����ϴٸ� 
                {
                    isDestructionPart = false;
                    damageMultiplier = partDestructionDamageMultiplier;     // ������ ������ �����ı��� ������
                    skinRenderer.gameObject.SetActive(false);
                }

                if (!monsterAction.state.HasFlag(MONSTER_STATE.Stagger))
                {
                    monsterAction.state |= MONSTER_STATE.Stagger;
                    monsterAction.StartStaggerState();
                }

                currentHp = maxhp * 1.2f;
            }

            Debug.Log(gameObject.name + "�� ���� ü�� : " + currentHp);
        }
    }


    private void Start()
    {
        player = FindObjectOfType<PlayerStatus>();
        monster = transform.GetComponentInParent<DruidStatus>();
        monsterAction = transform.GetComponentInParent<MonsterAction>();
        Hp = maxhp;

        //Debug.Log("monster name : " + monster.name + ", monsterAction : " + monsterAction.name);
    }

    public void OnCollisionEnter(Collision collision)
    {
        //if ()                 // ���Ͱ� �������϶��� �浹�� ������ ������
        //{

        //}
    }
    // ü�� �� �޾����� ���� �Ͼ�� �Լ� �����, ������ ������� ��/��/��/Ư��, ü�����ϰ�, ������ ����
    // �÷��̾� ������ �������� �������ΰ�? -> ��ƼŬ�� ��ũ��Ʈ�� �ִ¹���� �Ұ���? 

    // �����Լ�, �ѹ��� �������� ���� �ݶ��̴��� �浹������ ������ ������ ���°� �����ϱ�
}
