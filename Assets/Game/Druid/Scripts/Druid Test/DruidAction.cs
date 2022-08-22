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

    public void SetDestinationDirection(Transform targetPos, float angleLimit = 0f)     // 목표의 방향과 회전애니메이션을 정해줌
    {
        // 몬스터가 얼마나 회전할지 각도 구하기
        Vector3 dir = targetPos.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // 목표지점은 네브메쉬(땅)이니깐 Y축을 0으로 함으로써 바닥을 보지 않도록 해줌.
        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);            // 내가 바라볼 방향
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);   // 내 방향과 목표 방향의 각도차이

        if (angleDifference <= angleLimit)                          // angleLimit 이하의 각도는 돌지 않는다.
        {
            druidStatus.state &= ~MONSTER_STATE.Rotation;
            rotationCoroutine = null;
            return;
        }

        // 몬스터가 어떤 방향으로 회전할지 구하기 
        Vector3 targetDir = targetPos.position - transform.position;                    // 타겟 방향으로 향하는 벡터를 구하기
        Vector3 crossVec = Vector3.Cross(targetDir, this.transform.forward);            // foward와 외적
        float dot = Vector3.Dot(crossVec, Vector3.up);                                  // 위방향과 내적
        if (dot > 0) // 왼쪽
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
        else if (dot < 0) // 오른쪽
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
        else // 가운데 (0일때)
        {
        }
    }

    public IEnumerator Rotation(string name, float targetAngle)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))
            yield return null;

        float exitTime = animator.GetCurrentAnimatorStateInfo(0).length;

        if (targetAngle < 60)
            exitTime = exitTime * 0.90f;    // 현재 실행된 회전 애니메이션의 90%시간동안 코루틴 실행(회전하는 모습이 어색하지 않도록)

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
