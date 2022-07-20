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
    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("HitAble"))
        {
            // 캐릭터에 데미지 넣는 함수 실행
            Debug.Log(collision.gameObject.name + "에 부딪힘");
            Destroy(gameObject);
        }
    }
}
