using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackablePart : MonoBehaviour
{
    protected Collider myCollider;
    protected MonsterStatus monster; // ���ݷ� �������� ����

    [SerializeField] protected float damageMultiplier;
    protected bool isAttackAble;
}
