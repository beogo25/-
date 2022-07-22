using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHitablePart : MonoBehaviour
{
    [SerializeField] private float maxhp;
    [SerializeField] private float damageMultiplier;
    [SerializeField] private bool isDestructionPart;
    [SerializeField] private float partDestructionDamageMultiplier;
    [SerializeField] private SkinnedMeshRenderer skinRenderer;
    
    private float currentHp;
    private MonsterStatus monster;
    private PlayerStatus player;
    public float Hp
    {
        get { return currentHp; }
        set 
        { 
            currentHp = value;
            Debug.Log(gameObject.name + "의 현재 체력 : " + currentHp);
            if (currentHp <= 0)
            {
                currentHp = 0;
                if(isDestructionPart)   // 부위파괴가 가능하다면 
                {
                    isDestructionPart = false;
                    damageMultiplier = partDestructionDamageMultiplier;     // 데미지 배율을 부위파괴시 배율로
                    skinRenderer.gameObject.SetActive(false);
                }
            }
        }
    }
    
    private void Start()
    {
        player = FindObjectOfType<PlayerStatus>();
        monster = FindObjectOfType<MonsterStatus>();
        Hp = maxhp;
    }
    public void Hit(float damage)
    {
        Hp -= damage * damageMultiplier;
        monster.Hp -= damage * damageMultiplier;
        //Debug.Log("hit 실행" +  damage * damageMultiplier);
    }
    public void OnCollisionEnter(Collision collision)
    {
        //if ()                 // 몬스터가 공격중일때만 충돌시 데미지 들어가도록
        //{

        //}
    }
    // 체력 다 달았을시 경직 일어나는 함수 만들기, 부위별 경직모션 소/중/대/특대, 체력정하고, 데미지 배율
    // 플레이어 공격은 데미지가 고정값인가? -> 파티클에 스크립트를 넣는방식을 할건지? 

    // 경직함수, 한번의 공격으로 여러 콜라이더가 충돌했을시 데미지 여러번 들어가는거 방지하기


}
