using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MONSTER_BEHAVIOR_STATE
{
    SerchingTarget,
    InBattle,
}
public enum MONSTER_ATTACK_TYPE
{
    Stomp,
    Swipe,
    JumpAttack,
    Roar,
    Throw
}

public class Monster : MonoBehaviour
{
    private enum State
    {
        Idle = 0,
        Walk = 1 << 0,
        Rotation = 1 << 1,
        Attack = 1 << 2,

        All = Walk | Rotation | Attack,
    }
    private MONSTER_BEHAVIOR_STATE behaviorState;
    private State state = State.Idle;

    private Collider target;
    private Animator animator;
    private Rigidbody monsterRigidbody;
    
    [SerializeField] private Transform moveDestination;
    private bool randomAnimationCoolTime = false;
    private float moveSpeed = 3;
    
    [Header("시야각 관련"),Space(10)]
    [SerializeField] private bool DebugMode = true;
    private float ViewAngle = 45f;
    private float ViewRadius = 30f;
    private LayerMask ObstacleMask;

    [Header("전투관련"), Space(10)]
    [SerializeField] private GameObject[] throwingObject;
    [SerializeField] private Collider[] bodyCollider;
    [SerializeField] private Transform rightHand;
    [SerializeField] private GameObject attackParticle;
    void Start()
    {
        animator = GetComponent<Animator>();
        monsterRigidbody = GetComponent<Rigidbody>();
        
        StartCoroutine(CoolTime(Random.Range(10f, 20f),"RandomAnimation"));
        behaviorState = MONSTER_BEHAVIOR_STATE.SerchingTarget;
        StartCoroutine(behaviorState.ToString());
    }
    private void Update()
    {
        if (behaviorState == MONSTER_BEHAVIOR_STATE.SerchingTarget && target == null)
        {
            if (FindTarget())
            {
                ChangeState(MONSTER_BEHAVIOR_STATE.InBattle);
            }
        }
        else if (behaviorState == MONSTER_BEHAVIOR_STATE.InBattle && target != null)
        {
            if (CheckTargetIsInArea())
                ChangeState(MONSTER_BEHAVIOR_STATE.SerchingTarget);
        }



        // 테스트 부분

        //if (target != null)
        //{
        //    //transform.LookAt(target.transform);
        //    Vector3 targetD = (target.transform.position - transform.position).normalized;
        //    targetD = new Vector3(targetD.x, 0, targetD.z);                         // 목표지점은 네브메쉬(땅)이니깐 Y축을 0으로 함으로써 바닥을 보지 않도록 해줌.
        //    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetD.normalized), 2f * Time.deltaTime);

        //    transform.Translate(targetD * moveSpeed * Time.deltaTime, Space.World);

        //}
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    StartCoroutine(RangeAttack("Rock"));
        //}
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    StartCoroutine(RangeAttack("Wood"));
        //}
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (target == null)
            {
                Debug.Log("타겟없음");
            }
            else {
                Debug.Log("타겟 : " + target.name);
                //SetDestinationDirection(target.transform);
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("현재 state : " + state);
            //animator.SetTrigger("JumpAttack");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            DebugMode = !DebugMode;
            
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            //Debug.Log("랜덤밸루 : " + Random.value);
            //state |= State.Attack;
            //animator.SetTrigger("Roar");
            
        }

    }

    #region SerchingTargetState
    private enum SerchingTargetState { Rotation, Walk };
    SerchingTargetState serchingTargetState = SerchingTargetState.Rotation;
    IEnumerator rotationCoroutine;
    Quaternion targetRotation;
    IEnumerator SerchingTarget()
    {
        while (true)
        {
            switch (serchingTargetState)
            {
                case SerchingTargetState.Rotation:
                    if (state == State.Idle)         // Rotation상태로 만들어줌 (상태가 바뀌었을때 한번만 실행되도록) 
                    {
                        state |= State.Rotation;
                        targetRotation = SetDestinationDirection(moveDestination);
                    }
                    
                    if (Quaternion.Angle(transform.rotation, targetRotation) < 4)   // 두 각의 차이가 일정(4도) 이하일때 목표를 보고 있는것으로 판단
                    {
                        serchingTargetState = SerchingTargetState.Walk;
                        //animator.SetInteger("Rotation", 0);
                    }

                    break;

                case SerchingTargetState.Walk:
                    if (Vector3.Distance(transform.position, moveDestination.position) < 4f)    // 목표와 거리가 4f 이내
                    {
                        monsterRigidbody.velocity = Vector3.zero;
                        if (CreateRandomDestination(moveDestination.position, 20f))
                        {
                            state &= ~State.Walk;
                            animator.SetBool("Walk", state.HasFlag(State.Walk));

                            yield return new WaitForSecondsRealtime(Random.Range(0.3f, 1.3f));  // 도착했을때 잠깐 대기시간 부여
                            serchingTargetState = SerchingTargetState.Rotation;
                        }
                    }
                    else
                    {
                        if (state == State.Idle)
                        {
                            state |= State.Walk;
                            animator.SetBool("Walk", state.HasFlag(State.Walk));
                        }

                        if (state.HasFlag(State.Walk))
                            transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);

                        if (randomAnimationCoolTime)
                        {
                            int random = Random.Range(0, 2);
                            if (random == 0)
                                yield return StartCoroutine(WaitForAnimation("Stretch", 1f));
                            else if (random == 1)
                                yield return StartCoroutine(WaitForAnimation("Look Around", 1f));
                            else
                                Debug.Log("행동안함");

                            randomAnimationCoolTime = false;                            // 실행되고 나서 false로 바꿔주기
                            StartCoroutine(CoolTime(Random.Range(10f, 20f), "RandomAnimation"));
                            
                        }
                    }
                    break;
            }

            yield return null;
        }
    }
    Quaternion SetDestinationDirection(Transform targetPos, float angleLimit = 360f)                            // 목적지 방향을 보게 하는 함수
    {        
        // 몬스터가 얼마나 회전할지 각도 구하기
        Vector3 dir = targetPos.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // 목표지점은 네브메쉬(땅)이니깐 Y축을 0으로 함으로써 바닥을 보지 않도록 해줌.
        targetRotation = Quaternion.LookRotation(dir.normalized);   // 내가 바라볼 방향
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);   // 내 방향과 목표 방향의 각도차이
        Debug.Log("각도차이 : " + angleDifference);
        if (angleDifference >= angleLimit)
        {
            state &= ~State.Rotation;
            return targetRotation;
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
                StartCoroutine(rotationCoroutine);
            }
            else
            {
                animator.SetInteger("Rotation", -1);
                rotationCoroutine = Rotation("Turn Left Slow", -angleDifference);
                StartCoroutine(rotationCoroutine);
            }
        }
        else if (dot < 0) // 오른쪽
        {
            if (angleDifference > 60)
            {
                animator.SetInteger("Rotation", 2);
                rotationCoroutine = Rotation("Turn Right", angleDifference);
                StartCoroutine(rotationCoroutine);
            }
            else
            {
                animator.SetInteger("Rotation", 1);
                rotationCoroutine = Rotation("Turn Right Slow", angleDifference);
                StartCoroutine(rotationCoroutine);
            }
        }
        else // 가운데 (0일때)
        {
        }

        return targetRotation;
    }
    IEnumerator Rotation(string name, float targetAngle)
    {
        
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))
            yield return null;

        float playTime = 0f;
        float exitTime = animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log("TartgetAngle : " + targetAngle);
        exitTime = exitTime * 0.30f;                        // 현재 실행된 회전 애니메이션의 30%시간동안 코루틴 실행(회전하는 모습이 어색하지 않도록)
        
        while (playTime < exitTime)
        {
            playTime += 0.01f;
            transform.Rotate(new Vector3(0, (targetAngle / (exitTime / 0.01f)), 0), Space.Self);
            yield return new WaitForSecondsRealtime(0.01f); ;
        }
        state &= ~State.Rotation;
        animator.SetInteger("Rotation", 0);
        //Debug.Log("로테이션 끝나고 값 : " + state);
    }

    bool CreateRandomDestination(Vector3 center, float range)
    {
        for (int i = 0; i < 40; i++)        // 조건을 만족하는 목적지가 나올때까지 시행 (최대 40회)
        {
            Vector3 CreateRandomDestination = center + Random.insideUnitSphere * range;
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(CreateRandomDestination, out hit, 1.5f, NavMesh.AllAreas) && Vector3.Distance(transform.position, CreateRandomDestination) > 7f)
            {
                moveDestination.position = hit.position;
                return true;
            }
        }
        return false;
    }
    private bool FindTarget()
    {
        NavMeshHit hit;
        Collider[] targets = Physics.OverlapSphere(transform.position, ViewRadius, 1<< LayerMask.NameToLayer("Player"));

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 targetPos = targets[i].transform.position;
            Vector3 targetDir = (targetPos - transform.position).normalized;
            float targetAngle = Mathf.Acos(Vector3.Dot(AngleToDir(transform.eulerAngles.y), targetDir)) * Mathf.Rad2Deg;
            if (targetAngle <= ViewAngle * 0.5f && !Physics.Raycast(transform.position, targetDir, ViewRadius, ObstacleMask))
            {
                if (NavMesh.SamplePosition(targets[i].transform.position, out hit, 2.0f, NavMesh.AllAreas))
                {
                    target = targets[i];
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
    IEnumerator CoolTime(float cool, string func)
    {
        while (cool > 0)
        {
            cool -= Time.deltaTime;
            yield return null;
        }

        switch(func)
        {
            case "RandomAnimation":
                randomAnimationCoolTime = true;
                break;
            case "attackCoolTime":
                attackCoolTime = true;
                break;
        }
        
    }
    #endregion SerchingTargetState


    #region InBattle
    bool attackCoolTime = false;
    MONSTER_ATTACK_TYPE attackType;
    IEnumerator InBattle()
    {
        float targetDistance;
        
        InitState();                                                // 몬스터 상태 초기화
        yield return StartCoroutine(LookatTarget());                // 타겟 처다보기
        yield return StartCoroutine(WaitForAnimation("Roar",1f));   // 표효하기
        
        while (true)
        {
            if (DebugMode)
                Debug.Log("와일안 공격 타입 : " + attackType);
            targetDistance = Vector3.Distance(transform.position, target.transform.position);
            if (attackCoolTime && (state == State.Idle || state == State.Walk))
            {
                //Debug.Log("타겟과의 거리 : " + targetDistance);
                if (targetDistance < 25)
                {
                    state &= ~State.Walk;
                    animator.SetBool("Walk", false);
                    state |= State.Rotation | State.Attack;
                    SetDestinationDirection(target.transform);

                    if (targetDistance < 6.5f)     // 밟기 공격
                    {
                        if (Random.value <= 0.7f)
                            attackType = MONSTER_ATTACK_TYPE.Stomp;
                        else
                            attackType = MONSTER_ATTACK_TYPE.Swipe;
                        
                    }
                    else if (targetDistance < 8.5f)     // 할퀴기 공격
                    {
                        if (Random.value <= 0.7f)
                            attackType = MONSTER_ATTACK_TYPE.Swipe;
                        else
                            attackType = MONSTER_ATTACK_TYPE.Roar;
                    }
                    else if (targetDistance < 16)
                    {
                        if (Random.value <= 0.7f)
                            attackType = MONSTER_ATTACK_TYPE.JumpAttack;
                        else
                            attackType = MONSTER_ATTACK_TYPE.Roar;
                    }
                    else        //if (20 < targetDistance && targetDistance < 25) // 원거리 공격
                    {
                        if (Random.value <= 0.4f)
                            attackType = MONSTER_ATTACK_TYPE.Roar;
                        else
                            attackType = MONSTER_ATTACK_TYPE.Throw;
                    }
                }
                else        // 너무 멀어서 추격
                {
                    state |= State.Walk;
                }
            }
            else if(!attackCoolTime)        // 스킬 쿨일때
            {
                state |= State.Walk;
            }
            if (DebugMode)
                Debug.Log("현재 공격 타입 : " + attackType);
            if (state == State.Attack) // 
            {
                //Debug.Log("밟기 공격 시작 현재 상태 : " + state);
                switch (attackType)
                {
                    case MONSTER_ATTACK_TYPE.Stomp:
                        yield return StartCoroutine(WaitForAnimation("Stomp", 1f,true));
                        break;

                    case MONSTER_ATTACK_TYPE.Swipe:
                        yield return StartCoroutine(WaitForAnimation("Swipe", 1f, true));
                        break;

                    case MONSTER_ATTACK_TYPE.Roar:
                        yield return StartCoroutine(WaitForAnimation("Roar", 1f, true));
                        break;

                    case MONSTER_ATTACK_TYPE.JumpAttack:
                        yield return StartCoroutine(WaitForAnimation("JumpAttack", 1f, true));
                        break;

                    case MONSTER_ATTACK_TYPE.Throw:
                        momentTargetPosition = target.transform.position;   // 돌을 줍는 동작시 타겟 위치 
                        yield return StartCoroutine(WaitForAnimation("Throw",1f,true));
                        break;
                }
                state &= ~State.Attack;

                yield return new WaitForSecondsRealtime(1f);
                if (Random.value < 0.7f)
                    yield return StartCoroutine(WaitForAnimation("Flexing Muscle", 1f));
            }

            if(state.HasFlag(State.Walk))
            {
                if (targetDistance > 7f)
                {
                    ChaseTarget();
                }
                else
                {
                    state &= ~State.Walk;
                    animator.SetBool("Walk", false);
                    if(Random.value < 0.7f)
                        yield return StartCoroutine(WaitForAnimation("Flexing Muscle", 1f));
                }
            }

            yield return null;
        }
    }
    void InitState()
    {
        attackCoolTime = false;

        state = State.Idle;                                 
        animator.SetInteger("Rotation", 0);
        animator.SetBool("Walk", state.HasFlag(State.Walk));

        StartCoroutine(CoolTime(Random.Range(3f, 6f), "attackCoolTime"));   // 공격 쿨타임 돌기
        if (rotationCoroutine!=null)
            StopCoroutine(rotationCoroutine);
    }
    IEnumerator LookatTarget()
    {
        Vector3 dir = target.transform.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // 목표지점은 네브메쉬(땅)이니깐 Y축을 0으로 함으로써 바닥을 보지 않도록 해줌.
        targetRotation = Quaternion.LookRotation(dir.normalized);   // 내가 바라볼 방향

        while (Quaternion.Angle(transform.rotation, targetRotation) > 4)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
            yield return null;
        }
    }
    void ChaseTarget()
    {
        animator.SetBool("Walk", true);
        transform.LookAt(target.transform);
        Vector3 targetD = (target.transform.position - transform.position).normalized;
        targetD = new Vector3(targetD.x, 0, targetD.z);                         // 목표지점은 네브메쉬(땅)이니깐 Y축을 0으로 함으로써 바닥을 보지 않도록 해줌.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetD.normalized), 2f * Time.deltaTime);

        transform.Translate(targetD * moveSpeed * Time.deltaTime, Space.World);

    }
    float targetOutTime = 0;
    bool CheckTargetIsInArea()          
    {
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(target.transform.position, out hit, 2.0f, NavMesh.AllAreas)) // 캐릭터가 보스영역이 아니라면       // 타겟이 보스영역 밖으로 나갔을때 
        {
            targetOutTime += Time.deltaTime;

            if (targetOutTime > 3f)     // 3초이상 밖에 있을지 타겟해제
            {
                target = null;
                return true;
            }
        }
        else    // 캐릭터가 보스 영역 안에 있을 경우
        {
            targetOutTime = 0;
        }
        
        return false;
    }
    #region 애니메이션 이벤트
    Projectile projectile;
    Vector3 momentTargetPosition;
    void RangeAttack(int value) // 애니메이션 이벤트 (돌던지기일때만 발동)
    {
        switch (value)
        {
            case 0:     // 돌던지기
                projectile = Instantiate(throwingObject[0], rightHand.position + rightHand.right, transform.rotation, rightHand).GetComponent<Rock>();
                
                Debug.Log("던지ㅣ실행");
                break;

            case 1:     // 나무휘두르기
                projectile = Instantiate(throwingObject[1], rightHand.position - rightHand.up*1.5f, transform.rotation, rightHand).GetComponent<Wood>();
                animator.SetTrigger("Swing");

                break;
        }
    }
    void ThrowProjectile()     // 애니메이션 이벤트 (돌던지기일때만 발동)
    {
        if (projectile is Rock)
        {
            projectile.Init(momentTargetPosition, 10, 25);   // 매개변수 (타겟의 위치, 공격력, 속도)
            //rightHand.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
        else if(projectile is Wood)
        {
            projectile.Init((transform.right*2 - transform.up), 15, 50);   // 매개변수 (타겟의 위치, 공격력, 속도)
            //rightHand.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
    }
    void Roar()
    {
        Debug.Log("state : " + state);
        if (state.HasFlag(State.Attack))
        {
            attackParticle.SetActive(true);
        }
        Debug.Log("끝 state : " + state);
    }
    #endregion 애니메이션 이벤트

    #endregion InBattle
    public void ChangeState(MONSTER_BEHAVIOR_STATE newState)
    {
        StopCoroutine(behaviorState.ToString());                // 기존 실행중인 코루틴 정지
        behaviorState = newState;
        Debug.Log("ChangeState! 바뀔 상태 : " + newState);
        StartCoroutine(behaviorState.ToString());               // 변경된 상태로 코루틴 시작
    }
    IEnumerator WaitForAnimation(string name, float exitRatio, bool isAttackAni = false, int layer = -1)
    {
        float playTime =0;
        animator.Play(name, layer, 0);  // layer에 name이름을 가진 애니메이션을 0초부터 시작해라

        //animator.SetTrigger(name);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))   // 애니메이션이 전환될때까지 대기
            yield return null;

        float exitTime = animator.GetCurrentAnimatorStateInfo(0).length * exitRatio;
        while (playTime < exitTime)
        {
            playTime += Time.deltaTime;
            yield return null;
        }

        if (isAttackAni)   // 공격애니메이션의 동작이 끝났을때 
        {
            attackCoolTime = false;
            StartCoroutine(CoolTime(Random.Range(3f, 6f), "attackCoolTime"));
        }

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

