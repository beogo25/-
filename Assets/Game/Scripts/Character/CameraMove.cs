using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPoint;
    private GameObject player;

    private float      cameraX;
    private float      cameraY;

    private bool       isFocused;

    [SerializeField]
    [Range(0f, 10f)]
    private float      cameraZ;
    [SerializeField]
    private float      cameraSpeed;

    private Vector3    offset;
    private RaycastHit hit;
    private int        layerMask;
    

    private void Start()
    {
        player = playerPoint.transform.parent.gameObject;
        layerMask = (1 << LayerMask.NameToLayer("Wall") | LayerMask.NameToLayer("Ground"));
        
    }

    private void FixedUpdate()
    {
        CameraMovement();
    }

    private void CameraMovement()
    {

        //ī�޶� ��Ʈ�ѷ��� ������
        if (!isFocused)                                                                                 // ī�޶� ���̳� Ÿ�Ͽ��� ������ ������ isFocused �� ���
        {
            offset = new Vector3(0, 0, cameraZ);

            Debug.DrawRay(playerPoint.transform.position, transform.position - playerPoint.transform.position, Color.red);
            //ī�޶��� ���� �����Ʈ, ��ü�� ���� �� ī�޶� �տ� ��ġ�ȴ�
            if (Physics.Raycast(playerPoint.transform.position, transform.position - playerPoint.transform.position, out hit, Vector3.Distance(transform.position, playerPoint.transform.position), layerMask))
                Camera.main.transform.position = hit.point + (playerPoint.transform.position - hit.point).normalized * 0.5f;

            else
                Camera.main.transform.position = transform.position;

            cameraX += Input.GetAxis("Mouse X");
            cameraY -= Input.GetAxis("Mouse Y");
            cameraY  = Mathf.Clamp(cameraY, -20, 50);                                                   // �� �Ʒ��� ���� ����
            transform.rotation = Quaternion.Euler(cameraY, cameraX, 0);                                 // �̵����� ���� ī�޶��� �ٶ󺸴� ������ ����(�������� �� ������ �ʿ��� ����)
            transform.position = playerPoint.transform.position - transform.rotation * offset;          // �÷��̾��� ��ġ���� ī�޶� �ٶ󺸴� ���⿡ ���Ͱ��� ������ ��� ��ǥ�� ����
        }
    }

    public IEnumerator CameraFocus()
    {
        GameObject target = playerPoint.transform.parent.GetComponent<Player>().lockOnObject;           //Ÿ�� ��� ����
        isFocused = true;

        if (Player.isLockedOn)                                                                          //������ �Ǿ��ٸ�
        {
            Vector3 angleToTargetVec = target.transform.position - playerPoint.transform.position;      //Ÿ�ٿ��� �÷��̾��� ���͸� ���ϰ�
            float angleToTarget = Mathf.Atan2(angleToTargetVec.x, angleToTargetVec.z) * Mathf.Rad2Deg;  //��ũ ź��Ʈ�� ����(angle)�� ����

            cameraX = angleToTarget;                                                                    //�� ������ cameraX���� �־��ְ�,

            while (Mathf.Abs(angleToTarget - transform.rotation.eulerAngles.y) > 0.5)                   //angle�� ī�޶��� ������ ������������ �����ش�
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, angleToTarget, 0), cameraSpeed * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, playerPoint.transform.position - transform.rotation * offset, cameraSpeed * Time.deltaTime);
                if (Mathf.Abs(angleToTarget - transform.rotation.eulerAngles.y) > 359)                  //angle�� 360�� �Ǵ� ��쵵 �ֱ⿡ �̸� ���ش�
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
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(cameraY, cameraX, 0), cameraSpeed * Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, playerPoint.transform.position - transform.rotation * offset, cameraSpeed * Time.deltaTime);
                yield return new WaitForSeconds(0.005f);
            }
        }

        isFocused = false;
    }


}

