using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Projectile
{
    MeshCollider meshCollider;
    public override void Init(Vector3 target, float damage, float speed)
    {
        meshCollider = GetComponent<MeshCollider>();
        base.Init(target,damage,speed);
    }
    public override IEnumerator throwing(Vector3 targetPos)
    {
        Vector3 targetDir = (targetPos - transform.position).normalized; //목표의 위치를 잡고
        meshCollider.enabled = true;
        while (true)
        {
            transform.Rotate(7f, 3f, 0);
            transform.position += targetDir * speed * Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("HitAble")) return;
        if (other.gameObject.GetComponent<PlayerStatus>() != null)
        {
            other.transform.GetComponent<PlayerStatus>().PlayerHit(damage, 15, transform.position);
            Debug.Log("돌맞음 데미지 : " + damage);
        }

        Destroy(gameObject);
    }
}
