using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterMoveTest : MonoBehaviour
{
    enum State
    {
        Idle = 1 << 0,
        Walk = 1 << 1,
        Rotation = 1 << 2,

        All = Walk | Rotation,
    }

    private MonsterBehaviorState behaviorState;
    private State state = 0;

    private Transform targetPlayer;
    private Animator animator;
    private Rigidbody monsterRigidbody;

    [SerializeField] public Transform moveDestination;
    [SerializeField] private float moveSpeed = 2;
    private float rotationSpeed;
    private Vector3 point = Vector3.zero;



    void Start()
    {
        animator = GetComponent<Animator>();
        monsterRigidbody = GetComponent<Rigidbody>();

        behaviorState = MonsterBehaviorState.SerchingTarget;
        StartCoroutine(behaviorState.ToString());
    }


    int SerchingTargetState = 0;
    Quaternion targetRotation;
    IEnumerator SerchingTarget()
    {
        while (true)
        {
            if (SerchingTargetState == 0)       // ��ǥ ������
            {
                bool rotate = true;


                //state |= State.Rotation;
                targetRotation = SetTargetDirection(out rotate);


                //if (rotate)
                //    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                if (Quaternion.Angle(transform.rotation, targetRotation) < 7)   // �� ���� ���̰� ����(7��) �����϶� ��ǥ�� ���� �ִ°����� �Ǵ�
                {
                    // SerchingTargetState = 1;
                    state &= ~State.Rotation;
                    animator.SetInteger("Rotation", 0);

                    Debug.Log("��ǥ ���� �Ϸ�");
                    //yield return new WaitForSecondsRealtime(0.3f);
                }

            }

            yield return null;
        }
    }
    Quaternion SetTargetDirection(out bool rotate)
    {
        //Debug.Log("��ǥ ���� ã�� ����");

        Vector3 dir = moveDestination.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        targetRotation = Quaternion.LookRotation(dir.normalized);



        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

        Vector3 targetDir = moveDestination.position - transform.position; // Ÿ�� �������� ���ϴ� ���͸� ���ϱ�
        Vector3 crossVec = Vector3.Cross(targetDir, this.transform.forward); // ������� ����
        float dot = Vector3.Dot(crossVec, Vector3.up);
        if (dot > 0) // ����
        {
            Debug.Log("����");
        }
        else if (dot < 0) // ������
        {
            Debug.Log("������");
        }
        else // 0 �̶�� �����ϱ� ������ ������ ����ξ���.
        {
            Debug.Log("���");
        }

        if (angleDifference > 120)
        {
            //Debug.Log("���� : " + angleDifference + ", ��ȯ�ӵ� : 2 ����");
            rotationSpeed = 1f;
            //Debug.LogFormat("�� ���� : {0}, ��ǥ ���� : {1}",  transform.rotation.y, targetRotation.y);


            if (targetRotation.y > transform.rotation.y)
            {
                //animator.SetInteger("Rotation", 2);
                Debug.Log("���� : " + angleDifference + ", ������ ��������! " + "��ǥ ���� : " + Quaternion.Euler(0, targetRotation.y, 0) + ", �� ���� : " + Quaternion.Euler(0, transform.rotation.y, 0));
                
            }
            else
            {
                Debug.Log("���� : " + angleDifference + ", ���� ��������! " + "��ǥ ���� : " + Quaternion.Euler(0, targetRotation.y, 0) + ", �� ���� : " + Quaternion.Euler(0, transform.rotation.y, 0));
                //animator.SetInteger("Rotation", -2);
            }
            rotate = true;
        }
        else if (angleDifference > 60)
        {
            //Debug.Log("���� : " + angleDifference + ", ��ȯ�ӵ� : 1.3 ����");
            rotationSpeed = 1f;

            if (targetRotation.y > transform.rotation.y)
            {
                Debug.Log("���� : " + angleDifference + ", ������ ��������!, " + "��ǥ ���� : " + Quaternion.Euler(0, targetRotation.y,0)  + ", �� ���� : " + Quaternion.Euler(0, transform.rotation.y, 0));
                //animator.SetInteger("Rotation", 2);
            }
            else
            {
                Debug.Log("���� : " + angleDifference + ", ���� ��������! " + "��ǥ ���� : " + Quaternion.Euler(0, targetRotation.y, 0) + ", �� ���� : " + Quaternion.Euler(0, transform.rotation.y, 0));
                //animator.SetInteger("Rotation", -2);
            }
            rotate = true;
        }
        else
        {
            //Debug.Log("���� : " + angleDifference + ", ��ȯ�ӵ� : 0.8 ����");
            rotationSpeed = 0.8f;

            if (targetRotation.y > transform.rotation.y)
            {
                Debug.Log("���� : " + angleDifference + ", ������ ��������! " + "��ǥ ���� : " + Quaternion.Euler(0, targetRotation.y, 0) + ", �� ���� : " + Quaternion.Euler(0, transform.rotation.y, 0));
                //animator.SetInteger("Rotation", 1);
                
            }
            else
            {
                Debug.Log("���� : " + angleDifference + ", ���� ��������! " + "��ǥ ���� : " + Quaternion.Euler(0, targetRotation.y, 0) + ", �� ���� : " + Quaternion.Euler(0, transform.rotation.y, 0));
                //animator.SetInteger("Rotation", -1);
            }

            rotate = true;
        }
        
        return targetRotation;
    }
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 40; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas) && Vector3.Distance(transform.position, randomPoint) > 6f)
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
    IEnumerator WaitForAnimation(string name, float ratio, int layer = -1)
    {
        //animator.Play(name, layer, 0);  // layer�� name�̸��� ���� �ִϸ��̼��� 0�ʺ��� �����ض�
        animator.SetTrigger(name);
        Debug.Log("��Ʈ��ġ ���༺��!, " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < ratio)
        {
            Debug.Log("��Ʈ��ġ ������ : " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            yield return new WaitForEndOfFrame();
        }
    }


    //ChangeState(WeaponState.AttackToTarget); 
    public void ChangeState(MonsterBehaviorState newState)
    {
        StopCoroutine(behaviorState.ToString());              // ���� �������� �ڷ�ƾ ����

        behaviorState = newState;
        //Debug.Log("ChangeState! �ٲ� ���� : " + newState);
        StartCoroutine(behaviorState.ToString());             // ����� ���·� �ڷ�ƾ ����
    }

}