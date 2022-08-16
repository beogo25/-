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

            if (currentHp <= 0)             // ü���� 0���ϰ� �ȴٸ�
            {
                if (isDestructionPart)      // �����ı��� �����ϴٸ� 
                {
                    isDestructionPart = false;
                    damageMultiplier = partDestructionDamageMultiplier;     // ������ ������ �����ı��� ������
                    skinRenderer.gameObject.SetActive(false);
                }

                //Debug.Log(gameObject.name + ", ����ü�� : " + currentHp);
                if (!((DruidStatus)monster).state.HasFlag(MONSTER_STATE.Stagger) && 
                    !((DruidStatus)monster).state.HasFlag(MONSTER_STATE.Dead))    // ���� �� ������°� �ƴϸ�
                {
                    Debug.Log("���� ����");
                    ((Druid_InBattle)druidAction[1]).StartStaggerState();                      // ��������Ű��
                }

                currentHp = maxhp * 1.2f;   // ü���� 0���� ������ �ִ�ü���� 20%�� �÷� ü�ºο�
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

        Debug.Log(gameObject.name + "�� ���� ü�� : " + currentHp+ ", ��üü�� : " + monster.Hp +", ������ : "+ damage * damageMultiplier);
    }

    // ü�� �� �޾����� ���� �Ͼ�� �Լ� �����, ������ ������� ��/��/��/Ư��, ü�����ϰ�, ������ ����
    // �÷��̾� ������ �������� �������ΰ�? -> ��ƼŬ�� ��ũ��Ʈ�� �ִ¹���� �Ұ���? 

    // �����Լ�, �ѹ��� �������� ���� �ݶ��̴��� �浹������ ������ ������ ���°� �����ϱ�
}
