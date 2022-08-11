using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethod
{
    //2D 환경에서 LookAt과 같은 효과를 내는 함수
    public static void LookAt2D(this Transform transform, Transform target)
    {
        float angle;
        angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    //카메라가 목표물을 따라가도록
    public static void FollowTarget(this Camera camera, Transform target, float distance, float lerpSpeed)
    {
        camera.transform.position = Vector3.Lerp(camera.transform.position, new Vector3(target.position.x, target.position.y, target.position.z + distance), lerpSpeed * Time.deltaTime);
    }

    //Vector 값 normalize 해서 호출
    public static Vector3 GetVector(this Transform transform, Transform target)
    {
        Vector3 vector = transform.position - target.position;
        vector.Normalize();
        return vector;
    }

    //! 이미지를 스프라이트로 컨버트
    public static Sprite ToSprite(this Texture2D texture2D)
    {
        Rect rect = new Rect(0, 0, texture2D.width, texture2D.height);
        return Sprite.Create(texture2D, rect, new Vector2(0, 0), 1);
    }

    //카메라와 오브젝트를 비교해서 방향 구하기
    public static Vector3 GetDirectionByCamera(this Vector3 direction, Camera camera)
    {
        Vector2 moveInput   = new Vector2 (Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Vector3 lookForward = new Vector3 (camera.transform.forward.x, 0f, camera.transform.forward.z).normalized;
        Vector3 lookRight   = new Vector3 (camera.transform.right.x  , 0f, camera.transform.right.z  ).normalized;
        direction           = lookForward * moveInput.y + lookRight * moveInput.x;

        return direction.normalized;
    }

    //카메라와 오브젝트를 비교해서 방향 구하기(Raw값)
    public static Vector3 GetDirectionByCameraRaw(this Vector3 direction, Camera camera)
    {
        Vector2 moveInput   = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Vector3 lookForward = new Vector3 (camera.transform.forward.x, 0f, camera.transform.forward.z).normalized;
        Vector3 lookRight   = new Vector3 (camera.transform.right.x,   0f, camera.transform.right.z  ).normalized;
        direction           = lookForward * moveInput.y + lookRight * moveInput.x;

        return direction.normalized;
    }
}
