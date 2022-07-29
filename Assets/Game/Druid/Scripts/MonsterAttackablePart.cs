using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackablePart : MonoBehaviour
{
    protected Collider myCollider;
    protected MonsterStatus monster; // 공격력 가져오기 위함

    [SerializeField] protected float damageMultiplier;
    protected bool isAttackAble;
}
