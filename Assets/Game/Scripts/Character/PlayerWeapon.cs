using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private Player player;
    private BoxCollider boxCollider;

    public bool attackAble = false;
    public int attackNum;

    private float attackDamagePercent;
    private GameObject[] hitEffect;
    private void Awake()
    {
        player = transform.GetComponentInParent<Player>();
        boxCollider = transform.GetComponent<BoxCollider>();
    }

    public void InitWeapon(float attackDamagePercent,GameObject[] hitEffect)
    {
        this.attackDamagePercent = attackDamagePercent;
        this.hitEffect = hitEffect;
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name + "�̶� �浹11111");
        if (other.GetComponent<MonsterHitablePart>() != null && attackAble)
        {
            Debug.Log(other.name + ", �浹�ð� : " + Time.time);
            attackAble = false;
            boxCollider.enabled = false;
            other.GetComponent<MonsterHitablePart>().Hit(player.attackValue * attackDamagePercent);

            if (hitEffect.Length > 0)
            {
                hitEffect[attackNum].transform.position = other.ClosestPointOnBounds(player.transform.position);
                //collision.contacts[0].point
                hitEffect[attackNum].SetActive(true);
            }
        }
    }
}
