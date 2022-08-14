using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Druid_SerchingTarget : DruidAction
{

    [Header("이동 관련"), Space(10)]
    [SerializeField] private Transform moveDestination;
    private float moveSpeed = 3;

    [Header("시야각 관련"), Space(10)]
    [SerializeField] private bool DebugMode = true;
    private float ViewAngle = 45f;
    private float ViewRadius = 30f;
    private LayerMask ObstacleMask;

    private bool randomAnimationCoolTime = false;
    Rigidbody monsterRigidbody;

    private new void Awake()
    {
        base.Awake();
        monsterRigidbody = GetComponent<Rigidbody>();
    }
    void Start() => Init();
    
    public override void Init()     // 이 상태로 바뀌었을때 처음에 시작됨
    {
        Debug.Log("적찾는중 Init");
        druidStatus.state = MONSTER_STATE.Rotation;
        animator.SetInteger("Rotation", 0);
        animator.SetBool("Walk", druidStatus.state.HasFlag(MONSTER_STATE.Walk));

        animator.SetFloat("Breath Speed", 1f);                      // Idle상태(숨쉬기)의 속도를 원래대로 재생
        
        StartCoroutine(CoolTime(Random.Range(10f, 20f)));           // 랜덤애니메이션 쿨타임돌리기
        StartCoroutine(druidStatus.behaviorState.ToString());       // 드루이드 행동시작
    }
    void Update()
    {
        if (druidStatus.behaviorState != MONSTER_BEHAVIOR_STATE.SerchingTarget) return;     // SerchingTarget 상태가 아닐경우 update 실행X

        if (FindTarget())
        {
            Debug.Log("적탐지완료");
            ChangeState(MONSTER_BEHAVIOR_STATE.InBattle);
        }

    }
    #region 타겟 감지
    bool FindTarget()
    {
        NavMeshHit hit;
        Collider[] targets = Physics.OverlapSphere(transform.position, ViewRadius, 1 << LayerMask.NameToLayer("Character"));

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 targetPos = targets[i].transform.position;
            Vector3 targetDir = (targetPos - transform.position).normalized;
            float targetAngle = Mathf.Acos(Vector3.Dot(AngleToDir(transform.eulerAngles.y), targetDir)) * Mathf.Rad2Deg;
            if (targetAngle <= ViewAngle * 0.5f && !Physics.Raycast(transform.position, targetDir, ViewRadius, ObstacleMask))
            {
                if (NavMesh.SamplePosition(targets[i].transform.position, out hit, 2.0f, NavMesh.AllAreas))
                {
                    druidStatus.target = targets[i];
                    return true;
                }

                if (DebugMode) Debug.DrawLine(transform.position, targetPos + Vector3.up, Color.red);
            }
        }
        return false;
    }
    Vector3 AngleToDir(float angle)
    {
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(radian), 0f, Mathf.Cos(radian));
    }


    #endregion 타겟 감지
    //IEnumerator rotationCoroutine;
    IEnumerator SerchingTarget()
    {
        float walkTime = 0;

        while (true)
        {

            walkTime = 0;
            if (druidStatus.state == MONSTER_STATE.Rotation)            // Rotation상태로 만들어줌 (상태가 바뀌었을때 한번만 실행되도록) 
            {
                SetDestinationDirection(moveDestination);               // 목표방향에 따른 회전애니메이션

                yield return StartCoroutine(rotationCoroutine);
                druidStatus.state = MONSTER_STATE.Walk;
            }


            if (druidStatus.state == MONSTER_STATE.Walk)
            {
                if (Vector3.Distance(transform.position, moveDestination.position) < 8f)    // 목표와 거리가 8f 이내
                {
                    monsterRigidbody.velocity = Vector3.zero;
                    if (CreateRandomDestination(moveDestination.position, 20f))
                    {
                        druidStatus.state &= ~MONSTER_STATE.Walk;
                        animator.SetBool("Walk", druidStatus.state.HasFlag(MONSTER_STATE.Walk));

                        yield return new WaitForSecondsRealtime(0.3f);  // 도착했을때 잠깐 대기시간 부여
                        druidStatus.state = MONSTER_STATE.Rotation;
                    }
                }
                else
                {
                    walkTime += Time.deltaTime;

                    if (druidStatus.state.HasFlag(MONSTER_STATE.Walk))
                        transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
                    animator.SetBool("Walk", druidStatus.state.HasFlag(MONSTER_STATE.Walk));

                    if (randomAnimationCoolTime && walkTime > 1f)
                    {
                        float random = Random.value;
                        if (random <= 0.2f)
                            yield return StartCoroutine(WaitForAnimation("Stretch", 1f));
                        else if (random <= 0.6f)
                            yield return StartCoroutine(WaitForAnimation("Look Around", 1f));
                        else
                            Debug.Log("행동안함");

                        randomAnimationCoolTime = false;                            // 실행되고 나서 false로 바꿔주기
                        StartCoroutine(CoolTime(Random.Range(10f, 20f)));
                    }
                }
            }

            yield return null;
        }
    }
    bool CreateRandomDestination(Vector3 center, float range)
    {
        for (int i = 0; i < 40; i++)        // 조건을 만족하는 목적지가 나올때까지 시행 (최대 40회)
        {
            Vector3 CreateRandomDestination = center + Random.insideUnitSphere * range;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(CreateRandomDestination, out hit, 1.5f, NavMesh.AllAreas) && Vector3.Distance(transform.position, CreateRandomDestination) > 10f)
            {
                moveDestination.position = hit.position;
                return true;
            }
        }
        return false;
    }


    IEnumerator WaitForAnimation(string name, float exitRatio, int layer = -1)
    {
        float playTime = 0;
        //animator.Play(name, layer, 0);  // layer에 name이름을 가진 애니메이션을 0초부터 시작해라
        animator.SetTrigger(name);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))   // 애니메이션이 전환될때까지 대기
            yield return null;

        float exitTime = animator.GetCurrentAnimatorStateInfo(0).length * exitRatio;
        while (playTime < exitTime)
        {
            playTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
    }
    IEnumerator CoolTime(float cool)
    {
        while (cool > 0)
        {
            cool -= Time.deltaTime;
            yield return null;
        }

        randomAnimationCoolTime = true;
    }

    private void OnDrawGizmos()
    {
        if (!DebugMode) return;
        Vector3 myPos = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(myPos, ViewRadius);

        float lookingAngle = transform.eulerAngles.y;  //캐릭터가 바라보는 방향의 각도
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        Vector3 lookDir = AngleToDir(lookingAngle);

        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, lookDir * ViewRadius, Color.green);
    }
}

/*
    void SetDestinationDirection(Transform targetPos, float angleLimit = 0f)    // 목적지 방향을 보게 하는 함수
    {
        // 몬스터가 얼마나 회전할지 각도 구하기
        Vector3 dir = targetPos.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // 목표지점은 네브메쉬(땅)이니깐 Y축을 0으로 함으로써 바닥을 보지 않도록 해줌.
        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);            // 내가 바라볼 방향
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);   // 내 rotation과 목표 rotation의 각도차이

        if (angleDifference <= angleLimit)                          // angleLimit 이하의 각도는 회전하지 않는다.
        {
            druidStatus.state &= ~MONSTER_STATE.Rotation;
            rotationCoroutine = null;
            return;
        }

        // 몬스터가 어떤 방향으로 회전할지 구하기 (오른쪽 or 왼쪽)
        Vector3 targetDir = targetPos.position - transform.position;                    // 타겟 방향으로 향하는 벡터를 구하기
        Vector3 crossVec = Vector3.Cross(targetDir, this.transform.forward);            // foward와 외적
        float dot = Vector3.Dot(crossVec, Vector3.up);                                  // 위방향과 내적
        if (dot > 0)        // 왼쪽
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
        else if (dot < 0)   // 오른쪽
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
    IEnumerator Rotation(string name, float targetAngle)
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
 */