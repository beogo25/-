using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Projectile
{
    Rigidbody rb;
    public override void Init(Vector3 target, float damage, float speed)
    {
        rb = GetComponent<Rigidbody>();
        base.Init(target, damage, speed);
    }
    public override IEnumerator throwing(Vector3 targetPos)
    {
        Vector3 targetDir = (targetPos - transform.position).normalized;
        rb.isKinematic = false;
        while (true)
        {
            transform.Rotate(3, 12, 5);                       
            transform.position += targetDir * speed * Time.deltaTime;
            yield return null;
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log(collision.gameObject.name + " ºÎµúÈû");
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log(collision.gameObject.name + " ¶¥¿¡ ºÎµúÇô¤Å¼­ »ç¶óÁü");
            Destroy(gameObject);
        }
    }

}
