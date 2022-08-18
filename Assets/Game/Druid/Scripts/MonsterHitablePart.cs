using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterHitablePart : MonoBehaviour
{
    [SerializeField] protected string partName;                     // ���� �̸�
    [SerializeField] protected float maxhp;                         // �ִ� ü��(����ġ)
    [SerializeField] protected float damageMultiplier;              // ������ ���

    [Header("�����ı� ���� �ɼ�"), Space(10)]
    [SerializeField] protected bool  isDestructionPart;                 // ���İ����� ��������
    [SerializeField] protected float partDestructionDamageMultiplier;   // ���Ľ� ������ ���
    [SerializeField] protected SkinnedMeshRenderer skinRenderer;        // ������ ���� ������

    protected float currentHp;
    protected MonsterStatus monster;
    protected PlayerStatus  player;
    
    public virtual float Hp
    {
        get { return currentHp; }
        set { currentHp = value; }
    }

    public abstract void Hit(float damage);
    



}
