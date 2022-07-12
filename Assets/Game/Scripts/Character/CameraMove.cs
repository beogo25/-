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
    private float      ControllerZ;
    private float      cameraZ;
    [SerializeField]
    private float      cameraSpeed;
    
    private Vector3    offset;
    private Vector3    offsetCam;
    RaycastHit hit;


    private void LateUpdate()
    {
        CameraMovement();
    }

    private void CameraMovement()
    {
        offset    = new Vector3(0, 0, ControllerZ);
        offsetCam = new Vector3(0, 0, cameraZ);

        //ī�޶��� ���� �����Ʈ, ��ü�� ���� �� ī�޶� �տ� ��ġ�ȴ�
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Player") && hit.collider.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            {
                cameraZ = -hit.point.z;
                Camera.main.transform.position = player.transform.position - Camera.main.transform.rotation * offsetCam;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {

            }
            else
            {
                cameraZ =  ControllerZ;
                Camera.main.transform.position = transform.position;
            }
        }

        //ī�޶� ��Ʈ�ѷ��� ������
        if (!isFocused)                                                                   // ī�޶� ���̳� Ÿ�Ͽ��� ������ ������ isFocused �� ���
        {
            cameraX += Input.GetAxis("Mouse X");
            cameraY -= Input.GetAxis("Mouse Y");
            cameraY  = Mathf.Clamp(cameraY, -20, 50);                                     // �� �Ʒ��� ���� ����
            transform.rotation = Quaternion.Euler(cameraY, cameraX, 0);                   // �̵����� ���� ī�޶��� �ٶ󺸴� ������ ����(�������� �� ������ �ʿ��� ����)
            transform.position = player.transform.position - transform.rotation * offset; // �÷��̾��� ��ġ���� ī�޶� �ٶ󺸴� ���⿡ ���Ͱ��� ������ ��� ��ǥ�� ����
        }
    }

    public IEnumerator CameraFocus()
    {
        GameObject target = player.transform.parent.GetComponent<Player>().lockOnObject;                //Ÿ�� ��� ����
        isFocused = true;

        if (Player.isLockedOn)                                                                          //������ �Ǿ��ٸ�
        {
            Vector3 angleToTargetVec = target.transform.position - player.transform.position;           //Ÿ�ٿ��� �÷��̾��� ���͸� ���ϰ�
            float angleToTarget = Mathf.Atan2(angleToTargetVec.x, angleToTargetVec.z) * Mathf.Rad2Deg;  //��ũ ź��Ʈ�� ����(angle)�� ����

            cameraX = angleToTarget;                                                                    //�� ������ cameraX���� �־��ְ�,

            while (Mathf.Abs(angleToTarget - transform.rotation.eulerAngles.y) > 0.5)                   //angle�� ī�޶��� ������ ������������ �����ش�
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, angleToTarget, 0), cameraSpeed * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, player.transform.position - transform.rotation * offset, cameraSpeed * Time.deltaTime);
                if (Mathf.Abs(angleToTarget - transform.rotation.eulerAngles.y) > 359)                  //angle�� 360�� �Ǵ� ��쵵 �ֱ⿡ �̸� ���ش�
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