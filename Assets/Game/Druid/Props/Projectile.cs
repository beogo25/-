using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected float damage;
    protected float speed;

    protected virtual void Start()
    {
        Debug.Log("����ü 5�ʵ� �����");
        Destroy(gameObject, 5f);
    }
    public virtual void Init(Transform target,float damage, float speed)
    {
        Debug.Log("Init ����");
        this.damage = damage;
        this.speed = speed;
        transform.parent = null;
        StartCoroutine(throwing(target.position));
    }
    public abstract IEnumerator throwing(Vector3 targetPos);
    public abstract void OnCollisionEnter(Collision collision);
    

    
}
