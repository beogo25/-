using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    protected float damage;
    protected float speed;

    protected virtual void Start()
    {
        Destroy(gameObject, 5f);
    }
    public virtual void Init(Vector3 target,float damage, float speed)
    {
        this.damage = damage;
        this.speed = speed;
        transform.parent = null;
        StartCoroutine(throwing(target));
    }
    public abstract IEnumerator throwing(Vector3 targetPos);
    public abstract void OnCollisionEnter(Collision collision);
    

    
}
