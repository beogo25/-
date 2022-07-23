using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackParticle : MonoBehaviour
{
    public  float        time = 0.5f;
    private Collider     myCollider;
    private bool         attackAble = false;
    private Player       player;
    private int          attackNum;

    public  float        attackDamagePercent;
    public  float        attackDelayTime;
    //public  float        attackNumber;
    public  GameObject[] hitEffect;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
        player     = FindObjectOfType<Player>();    
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
            attackNum          = i;
            attackAble         = true;
            myCollider.enabled = true;
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            myCollider.enabled = false;
            yield return new WaitForSeconds(attackDelayTime);
        }
    }

    IEnumerator TurnOff()
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
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
                //hitEffect[attackNum].transform.position = other.transform.position;
                hitEffect[attackNum].transform.position = other.ClosestPointOnBounds(player.transform.position);
                hitEffect[attackNum].SetActive(true);
            }
        }
    }
}
