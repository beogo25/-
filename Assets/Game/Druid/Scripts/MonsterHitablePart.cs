using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterHitablePart : MonoBehaviour
{
    [SerializeField] protected string partName;                     // 부위 이름
    [SerializeField] protected float maxhp;                         // 최대 체력(경직치)
    [SerializeField] protected float damageMultiplier;              // 데미지 배수

    [Header("부위파괴 관련 옵션"), Space(10)]
    [SerializeField] protected bool  isDestructionPart;                 // 부파가능한 부위인지
    [SerializeField] protected float partDestructionDamageMultiplier;   // 부파시 데미지 배수
    [SerializeField] protected SkinnedMeshRenderer skinRenderer;        // 부파할 부위 렌더러

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
