using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPoint;
    private GameObject player;
    private Player playerScript;

    private float      cameraX;
    private float      cameraY;

    private bool       isFocused;

    [SerializeField]
    [Range(0f, 10f)]
    private float      cameraZ;
    [SerializeField]
    [Range(20f, 300f)]
    private float      cameraSpeed;
    [SerializeField]
    [Range(0f, 20f)]
    private float      cameraTurnSpeed;

    private Vector3    offset;
    private RaycastHit hit;
    private int        layerMask;
    

    private void Start()
    {
        player    = playerPoint.transform.parent.gameObject;
        playerScript = player.GetComponent<Player>();
        layerMask = (1 << LayerMask.NameToLayer("Wall") | LayerMask.NameToLayer("Ground"));
        
    }

    private void FixedUpdate()
    {
        if(!playerScript.talkState)
            CameraMovement();
    }

    private void CameraMovement()
    {

        //카메라 컨트롤러의 움직임
        if (!isFocused)                                                                                 // 카메라를 앞이나 타켓에게 돌리는 동안은 isFocused 로 명시
        {
            offset = new Vector3(0, 0, cameraZ);
           
            //카메라의 대한 무브먼트, 물체가 있을 시 카메라가 앞에 배치된다
            if (Physics.Raycast(playerPoint.transform.position, transform.position - playerPoint.transform.position, out hit, Vector3.Distance(transform.position, playerPoint.transform.position), layerMask))
                Camera.main.transform.position = hit.point + (playerPoint.transform.position - hit.point).normalized * 0.5f;
            else
                Camera.main.transform.position = transform.position;

            cameraX += Input.GetAxis("Mouse X") * cameraSpeed * Time.deltaTime;
            cameraY -= Input.GetAxis("Mouse Y") * cameraSpeed * Time.deltaTime;
            cameraY  = Mathf.Clamp(cameraY, -15, 80);                                                   // 위 아래의 각도 제한
            transform.rotation = Quaternion.Euler(cameraY, cameraX, 0);                                 // 이동량에 따라 카메라의 바라보는 방향을 조정(실질적인 값 조정이 필요한 영역)
            transform.position = playerPoint.transform.position - transform.rotation * offset;          // 플레이어의 위치에서 카메라가 바라보는 방향에 벡터값을 적용한 상대 좌표를 차감
        }
    }

    public IEnumerator CameraFocus(GameObject targetInput=null)
    {
        GameObject target = playerPoint.transform.parent.GetComponent<Player>().lockOnObject;           // 타겟 대상 선정
        isFocused = true;
        if (targetInput != null)
            target = targetInput;
        if (Player.isLockedOn)                                                                          // 락온이 되었다면
        {
            Vector3 angleToTargetVec = target.transform.position - playerPoint.transform.position;      // 타겟에서 플레이어의 벡터를 구하고
            float angleToTarget = Mathf.Atan2(angleToTargetVec.x, angleToTargetVec.z) * Mathf.Rad2Deg;  // 아크 탄젠트로 각도(angle)를 구함

            cameraX = angleToTarget;                                                                    // 그 각도를 cameraX값에 넣어주고,

            while (Mathf.Abs(angleToTarget - transform.rotation.eulerAngles.y) > 0.5)                   // angle이 카메라의 각도와 같아질때까지 돌려준다
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, angleToTarget, 0), cameraTurnSpeed * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, playerPoint.transform.position - transform.rotation * offset, cameraTurnSpeed * Time.deltaTime);
                if (Mathf.Abs(angleToTarget - transform.rotation.eulerAngles.y) > 359)                  // angle이 360가 되는 경우도 있기에 이를 빼준다
                    break;
                yield return new WaitForSeconds(0.005f);
            }

        }
        else
        {
            cameraX = playerPoint.transform.rotation.eulerAngles.y;
            cameraY = playerPoint.transform.rotation.eulerAngles.x;

            while (Mathf.Abs(playerPoint.transform.rotation.eulerAngles.y - transform.rotation.eulerAngles.y) > 0.5)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(cameraY, cameraX, 0), cameraTurnSpeed * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, playerPoint.transform.position - transform.rotation * offset, cameraTurnSpeed * Time.deltaTime);
                yield return new WaitForSeconds(0.005f);
            }
        }

        isFocused = false;
    }
}

