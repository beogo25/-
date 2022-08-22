using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackParticle : MonoBehaviour
{
    private Collider     myCollider;
    //private PlayerWeapon weapon;

    public float time = 0.5f;
    private bool attackAble = false;
    private Player player;
    private int attackNum;

    public float attackDamagePercent;
    public float attackDelayTime;
    public GameObject[] hitEffect;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        player = transform.GetComponentInParent<Player>();
        //weapon = weapon.gameObject.GetComponent<PlayerWeapon>();
    }

    private void OnEnable()
    {
        StartCoroutine(TurnOff());
        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        for (int i = 0; i < hitEffect.Length; i++)
        {
            attackNum = i;
            attackAble = true;
            myCollider.enabled = true;
            yield return new WaitForFixedUpdate();

            myCollider.enabled = false;
            yield return new WaitForSeconds(attackDelayTime);
            
        }
    }

    IEnumerator TurnOff()
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
        for (int i = 0; i < hitEffect.Length; i++)
            hitEffect[i].SetActive(false);
        
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MonsterHitablePart>() != null && attackAble)
        {
            attackAble = false;
            //Debug.Log(other.name +"  "+ player.attackValue * attackDamagePercent);
            other.GetComponent<MonsterHitablePart>().Hit(player.attackValue * attackDamagePercent);

            if (hitEffect.Length > 0)
            {
                hitEffect[attackNum].transform.position = other.ClosestPointOnBounds(player.transform.position);
                hitEffect[attackNum].SetActive(true);
                //Debug.Log(other.name +"  "+ player.attackValue * attackDamagePercent);
            }
        }
    }
}
