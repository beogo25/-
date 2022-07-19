using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Projectile
{
    public override IEnumerator throwing(Vector3 targetPos)
    {
        Vector3 targetLocation = (targetPos - gameObject.transform.position).normalized; //��ǥ�� ��ġ�� ���

        gameObject.transform.Rotate(8f, 3f, 0);
        gameObject.transform.position += targetLocation * speed * Time.deltaTime;

        yield return null;
    }
    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log(collision.gameObject.name + " �ε���");
        }
    }

}
