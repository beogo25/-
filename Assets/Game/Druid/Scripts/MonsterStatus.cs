using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MonsterStatus : MonoBehaviour
{
    [SerializeField] protected Collider[] bodyCollider; // �̰� �ʿ��Ѱ�?
    public event Action HitDel;

    protected float hp;

    public float Hp
    {
        get { return hp; }
        set
        {
            hp = value;
        }
    }
   


}
