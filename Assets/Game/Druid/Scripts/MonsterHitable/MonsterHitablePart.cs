using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHitablePart : MonoBehaviour
{
    [SerializeField] private float damageMultiplier;
    [SerializeField] private float maxhp;
    private float currentHp;

    private MonsterStatus monster;
    private PlayerStatus player;

    public float Hp
    {
        get { return currentHp; }
        set 
        { 
            currentHp = value; 
            
            if (currentHp < 0)
                currentHp = 0;
        }
    }
    
    private void Start()
    {
        player = FindObjectOfType<PlayerStatus>();
        monster = FindObjectOfType<MonsterStatus>();
        currentHp = maxhp;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("PlayerAttack")) // ���̾�� ���� �ȸ���
        {
            Hp -= player.Atk * damageMultiplier;
            monster.Hp -= player.Atk * damageMultiplier;
        }
    }
    // ü�� �� �޾����� ���� �Ͼ�� �Լ� �����, ������ ������� ��/��/��/Ư��, ü�����ϰ�, ������ ����
    // �÷��̾� ������ �������� �������ΰ�? -> ��ƼŬ�� ��ũ��Ʈ�� �ִ¹���� �Ұ���? 


}
