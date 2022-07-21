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
        if(collision.gameObject.layer == LayerMask.NameToLayer("PlayerAttack")) // 레이어는 아직 안만듬
        {
            Hp -= player.Atk * damageMultiplier;
            monster.Hp -= player.Atk * damageMultiplier;
        }
    }
    // 체력 다 달았을시 경직 일어나는 함수 만들기, 부위별 경직모션 소/중/대/특대, 체력정하고, 데미지 배율
    // 플레이어 공격은 데미지가 고정값인가? -> 파티클에 스크립트를 넣는방식을 할건지? 


}
