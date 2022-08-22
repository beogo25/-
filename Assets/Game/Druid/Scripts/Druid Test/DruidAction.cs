using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MONSTER_BEHAVIOR_STATE
{
    SerchingTarget,
    InBattle,
}

public enum MONSTER_STATE
{
    Idle     = 0,           // 0
    Walk     = 1 << 0,      // 1
    Rotation = 1 << 1,      // 2
    Attack   = 1 << 2,      // 4
    Stagger  = 1 << 3,      // 8
    Dead     = 1 << 4,      // 16

    All = Walk | Rotation | Attack,
}

public abstract class DruidAction : MonoBehaviour
{
    
    protected DruidAction[] druidAction = new DruidAction[2];
    protected DruidStatus druidStatus;
    protected Animator animator;

    protected IEnumerator rotationCoroutine;

    protected void Awake()
    {
        druidAction[0] = GetComponent<Druid_SerchingTarget>();
        druidAction[1] = GetComponent<Druid_InBattle>();

        druidStatus = GetComponent<DruidStatus>();
        animator = GetComponent<Animator>();
    }

    public abstract void Init();
    public void ChangeState(MONSTER_BEHAVIOR_STATE newState)
    {
        StopAllCoroutines();
        this.enabled = false;

        druidStatus.behaviorState = newState;
        druidAction[(int)newState].enabled = true;
        druidAction[(int)newState].Init();
    }

    public void SetDestinationDirection(Transform targetPos, float angleLimit = 0f)     // ��ǥ�� ����� ȸ���ִϸ��̼��� ������
    {
        // ���Ͱ� �󸶳� ȸ������ ���� ���ϱ�
        Vector3 dir = targetPos.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);            // ���� �ٶ� ����
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);   // �� ����� ��ǥ ������ ��������

        if (angleDifference <= angleLimit)                          // angleLimit ������ ������ ���� �ʴ´�.
        {
            druidStatus.state &= ~MONSTER_STATE.Rotation;
            rotationCoroutine = null;
            return;
        }

        // ���Ͱ� � �������� ȸ������ ���ϱ� 
        Vector3 targetDir = targetPos.position - transform.position;                    // Ÿ�� �������� ���ϴ� ���͸� ���ϱ�
        Vector3 crossVec = Vector3.Cross(targetDir, this.transform.forward);            // foward�� ����
        float dot = Vector3.Dot(crossVec, Vector3.up);                                  // ������� ����
        if (dot > 0) // ����
        {
            if (angleDifference > 60)
            {
                animator.SetInteger("Rotation", -2);
                rotationCoroutine = Rotation("Turn Left", -angleDifference);
            }
            else
            {
                animator.SetInteger("Rotation", -1);
                rotationCoroutine = Rotation("Turn Left Slow", -angleDifference);
            }
        }
        else if (dot < 0) // ������
        {
            if (angleDifference >= 60)
            {
                animator.SetInteger("Rotation", 2);
                rotationCoroutine = Rotation("Turn Right", angleDifference);
            }
            else
            {
                animator.SetInteger("Rotation", 1);
                rotationCoroutine = Rotation("Turn Right Slow", angleDifference);
            }
        }
        else // ��� (0�϶�)
        {
        }
    }

    public IEnumerator Rotation(string name, float targetAngle)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))
            yield return null;

        float exitTime = animator.GetCurrentAnimatorStateInfo(0).length;

        if (targetAngle < 60)
            exitTime = exitTime * 0.90f;    // ���� ����� ȸ�� �ִϸ��̼��� 90%�ð����� �ڷ�ƾ ����(ȸ���ϴ� ����� ������� �ʵ���)

        float playTime = 0f;
        while (playTime < exitTime)
        {
            playTime += 0.02f;
            transform.Rotate(new Vector3(0, (targetAngle / (exitTime / 0.02f)), 0), Space.Self);

            yield return new WaitForFixedUpdate();
        }

        druidStatus.state &= ~MONSTER_STATE.Rotation;
        animator.SetInteger("Rotation", 0);
        rotationCoroutine = null;
    }

}
