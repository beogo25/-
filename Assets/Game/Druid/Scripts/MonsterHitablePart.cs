using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterHitablePart : MonoBehaviour
{
    [SerializeField] protected float maxhp;
    [SerializeField] protected float damageMultiplier;
    [SerializeField] protected bool isDestructionPart;
    [SerializeField] protected float partDestructionDamageMultiplier;
    [SerializeField] protected SkinnedMeshRenderer skinRenderer;

    protected float currentHp;
    protected MonsterStatus monster;
    protected MonsterAction monsterAction;
    protected PlayerStatus  player;
    
    public virtual float Hp
    {
        get { return currentHp; }
        set { currentHp = value; }
    }

    public abstract void Hit(float damage);
    



}
