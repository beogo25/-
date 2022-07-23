using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHitablePart : MonoBehaviour
{
    [SerializeField] private float maxhp;
    [SerializeField] private float damageMultiplier;
    [SerializeField] private bool isDestructionPart;
    [SerializeField] private float partDestructionDamageMultiplier;
    [SerializeField] private SkinnedMeshRenderer skinRenderer;
    
    private float currentHp;
    private MonsterStatus monster;
    private PlayerStatus player;
    public float Hp
    {
        get { return currentHp; }
        set 
        { 
            currentHp = value;
            Debug.Log(gameObject.name + "�� ���� ü�� : " + currentHp);
            if (currentHp <= 0)
            {
                currentHp = 0;
                if(isDestructionPart)   // �����ı��� �����ϴٸ� 
                {
                    isDestructionPart = false;
                    damageMultiplier = partDestructionDamageMultiplier;     // ������ ������ �����ı��� ������
                    skinRenderer.gameObject.SetActive(false);
                }
            }
        }
    }
    
    private void Start()
    {
        player = FindObjectOfType<PlayerStatus>();
        monster = FindObjectOfType<MonsterStatus>();
        Hp = maxhp;
    }
    public void Hit(float damage)
    {
        Hp -= damage * damageMultiplier;
        monster.Hp -= damage * damageMultiplier;
        //Debug.Log("hit ����" +  damage * damageMultiplier);
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
