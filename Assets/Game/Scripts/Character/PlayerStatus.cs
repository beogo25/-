using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private int maxHp=100;
    public int hp;
    public int atk = 50;
    public int def = 0;

    public int Hp
    {
        get { return hp; }
        set { hp = value; } 
    }
    private void Awake()
    {
        hp = maxHp;
    }

    void Update()
    {
        
    }
}
