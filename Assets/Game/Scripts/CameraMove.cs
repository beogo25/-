using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private float      cameraX;
    private float      cameraY;
    private bool       isFocused;
    [SerializeField][Range(0f, 10f)]
    private float      cameraZ;
    private Vector3    offset;
    [SerializeField]
    private float      cameraSpeed;

    private void Update()
    {
        offset = new Vector3(0, -3, cameraZ);
        CameraMovement();
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(CameraFocus());
    }

    private void CameraMovement()
    {
        if (!isFocused)                                                                    //카메라를 앞이나 타켓에게 돌리는 동안은 isFocused 로 명시
        {
            cameraX += Input.GetAxis("Mouse X");
            cameraY -= Input.GetAxis("Mouse Y");
            cameraY  = Mathf.Clamp(cameraY, -20, 50);                                     // 위 아래의 각도 제한
            transform.rotation = Quaternion.Euler(cameraY, cameraX, 0);                   // 이동량에 따라 카메라의 바라보는 방향을 조정(실질적인 값 조정이 필요한 영역)
            transform.position = player.transform.position - transform.rotation * offset; // 플레이어의 위치에서 카메라가 바라보는 방향에 벡터값을 적용한 상대 좌표를 차감
        }
    }

    private IEnumerator CameraFocus()
    {

        GameObject target = player.GetComponent<Player>().lockOnObject;                                 //타겟 대상 선정
        isFocused = true;

        if (Player.isLockedOn)                                                                           //락온이 되었다면
        {
            Vector3 angleToTargetVec = target.transform.position - player.transform.position;           //타겟에서 플레이어의 벡터를 구하고
            float angleToTarget = Mathf.Atan2(angleToTargetVec.x, angleToTargetVec.z) * Mathf.Rad2Deg;  //아크 탄젠트로 각도(angle)를 구함

            cameraX = angleToTarget;                                                                    //그 각도를 cameraX값에 넣어주고,

            while (Mathf.Abs(angleToTarget - transform.rotation.eulerAngles.y) > 0.5)                   //angle이 카메라의 각도와 같아질때까지 돌려준다
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, angleToTarget, 0), cameraSpeed * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, player.transform.position - transform.rotation * offset, cameraSpeed * Time.deltaTime);
                if (Mathf.Abs(angleToTarget - transform.rotation.eulerAngles.y) > 359)                  //angle이 360가 되는 경우도 있기에 이를 빼준다
                    break;
                yield return new WaitForSeconds(0.005f);
            }

        }
        else
        {
            cameraX = player.transform.rotation.eulerAngles.y;
            cameraY = player.transform.rotation.eulerAngles.x;

            while (Mathf.Abs(player.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 0.5)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(cameraY, cameraX, 0), cameraSpeed * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, player.transform.position - transform.rotation * offset, cameraSpeed * Time.deltaTime);
                yield return new WaitForSeconds(0.005f);
            }
        }

        isFocused = false;
    }


}