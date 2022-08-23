using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MonsterStatus : MonoBehaviour
{
    [SerializeField] protected Collider monsterInterActionCollider;
    [HideInInspector] public string monsterName;
    public Sprite druidIcon;
    protected float maxHp;
    protected float currentHp;
    protected float atk;
    public virtual float Hp
    {
        get { return currentHp; }
        set
        {
            currentHp = value;
        }
    }

    public virtual float Atk
    {
        get { return atk; }
        set
        {
            atk = value;
        }
    }



}
