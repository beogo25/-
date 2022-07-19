using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : Projectile
{
    MeshCollider meshCollider;
    protected override void Start()
    {
        base.Start();
        //meshCollider = GetComponent<MeshCollider>();
    }
    public override void Init(Transform target, float damage, float speed)
    {
        base.Init(target,damage,speed);
        //meshCollider.enabled = true;
    }
    public override IEnumerator throwing(Vector3 targetPos)
    {
        Vector3 targetLocation = (targetPos - gameObject.transform.position).normalized; //목표의 위치를 잡고

        while (true)
        {
            gameObject.transform.Rotate(7f, 3f, 0);
            gameObject.transform.position += targetLocation * speed * Time.deltaTime;
            yield return null;
        }
    }
    public override void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // 캐릭터에 데미지 넣는 함수 실행
            Destroy(gameObject);
        }
    }
}
