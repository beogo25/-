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


#region �ڵ���
/*

public class AttackParticle : MonoBehaviour
{ 
    //private Collider     myCollider;
    [SerializeField] private BoxCollider  weaponCollider;
    private PlayerWeapon weapon;

    public  float        time = 0.5f;
    private bool         attackAble = false;
    private Player       player;
    private int          attackNum;

    public  float        attackDamagePercent;
    public  float        attackDelayTime;
    public  GameObject[] hitEffect;

    private void Awake()
    {
        //myCollider = GetComponent<Collider>();
        player = FindObjectOfType<Player>();
        weapon = weapon.gameObject.GetComponent<PlayerWeapon>();
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
            weapon.enabled = true;
            yield return new WaitForFixedUpdate();
            //yield return new WaitForFixedUpdate(); // �������� �ǵ��� �κ��ΰ�?
            weapon.enabled = false;
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
                //collision.contacts[0].point
                hitEffect[attackNum].SetActive(true);
            }
        }
    }
}

 */

/* ���� �ݶ��̴� ����
    [SerializeField] private BoxCollider  weaponCollider;
    private PlayerWeapon weapon;

    public  float        time = 0.5f;
    public  float        attackDamagePercent;
    public  float        attackDelayTime;
    public  GameObject[] hitEffect;

    private void Awake()
    {
        Debug.Log("weaponCollider name : " + weaponCollider.name);
        weapon = weaponCollider.gameObject.GetComponent<PlayerWeapon>();
        
    }

    private void OnEnable()
    {
        weapon.InitWeapon(attackDamagePercent, hitEffect[0]);
        StartCoroutine(TurnOff());
        //StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        for (int i = 0; i < hitEffect.Length; i++)
        {
            Debug.Log("�ݶ��̴� �� �ð� : " + Time.time);
            weapon.attackNum = i;
            weapon.attackAble = true;
            weaponCollider.enabled = true;

            yield return new WaitForSeconds(attackDelayTime);
        }
    }

    IEnumerator TurnOff()
    {
        yield return new WaitForSeconds(time);
        weaponCollider.enabled = false;
        gameObject.SetActive(false);
    } 
 */
#endregion