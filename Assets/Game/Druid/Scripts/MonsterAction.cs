using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MONSTER_BEHAVIOR_STATE
{
    SerchingTarget,
    InBattle,
}

public enum MONSTER_STATE
{
    Idle = 0,
    Walk = 1 << 0,
    Rotation = 1 << 1,
    Attack = 1 << 2,
    Stagger = 1<<3,

    All = Walk | Rotation | Attack,
}
public class MonsterAction : MonoBehaviour
{
    private enum MONSTER_ATTACK_TYPE
    {
        Stomp,
        Swipe,
        JumpAttack,
        Roar,
        Throw,
        Run
    }

    [HideInInspector] public MONSTER_BEHAVIOR_STATE behaviorState;
    [HideInInspector] public MONSTER_STATE state = MONSTER_STATE.Idle;

    [HideInInspector] public Collider target;
    private Animator animator;
    private Rigidbody monsterRigidbody;
    

    [SerializeField] private Transform moveDestination;
    private bool randomAnimationCoolTime = false;
    private float moveSpeed = 3;
    
    [Header("�þ߰� ����"),Space(10)]
    [SerializeField] private bool DebugMode = true;
    private float ViewAngle = 45f;
    private float ViewRadius = 30f;
    private LayerMask ObstacleMask;

    [Header("���� ����"), Space(10)]
    [SerializeField] private GameObject[] throwingObject;
    //[SerializeField] private MonsterHitablePart[] hitablePart;        // �ǰݺ��� ����
    [SerializeField] private DruidAttackablePart[] attackablePart;    // ���ݺ��� ����
    [SerializeField] private Transform  rightHand;
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



        // �׽�Ʈ �κ�
        if (Input.GetKeyDown(KeyCode.U))
        {
            if (target == null)
            {
                Debug.Log("Ÿ�پ���");
            }
            else {
                Debug.Log("Ÿ�� : " + target.name);
                //SetDestinationDirection(target.transform);
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            DebugMode = !DebugMode;
            //momentTargetPosition = Vector3.zero;
            //StartCoroutine(WaitForAnimation("Throw", 1f, true));
            //animator.Play("Dying(Front Up)", -1, 0);  // layer�� name�̸��� ���� �ִϸ��̼��� 0�ʺ��� �����ض�
            //animator.SetFloat("Breath Speed", 3f);
            Debug.Log("����");
            StartCoroutine(WaitForAnimation("Dying(Front Up)", 1));
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("���� state : " + state);

        }

    }

    #region SerchingTargetState
    private enum SerchingTargetState { Rotation, Walk };
    SerchingTargetState serchingTargetState = SerchingTargetState.Rotation;
    IEnumerator rotationCoroutine;
    
    IEnumerator SerchingTarget()
    {
        float walkTime = 0;
        InitState(behaviorState);   
        while (true)
        {
            //Debug.Log("switch ������ state : " + state);
            switch (serchingTargetState)
            {
                case SerchingTargetState.Rotation:
                    walkTime = 0f;
                    //Debug.Log("Rotation ������ state : " + state);
                    if (state == MONSTER_STATE.Idle)         // Rotation���·� ������� (���°� �ٲ������ �ѹ��� ����ǵ���) 
                    {
                        //Debug.Log("Rotation ���� state : " + state);
                        state |= MONSTER_STATE.Rotation;
                        SetDestinationDirection(moveDestination);
                        yield return StartCoroutine(rotationCoroutine);
                    }
                    break;

                case SerchingTargetState.Walk:
                    if (Vector3.Distance(transform.position, moveDestination.position) < 8f)    // ��ǥ�� �Ÿ��� 8f �̳�
                    {
                        monsterRigidbody.velocity = Vector3.zero;
                        if (CreateRandomDestination(moveDestination.position, 20f))
                        {
                            state &= ~MONSTER_STATE.Walk;
                            animator.SetBool("Walk", state.HasFlag(MONSTER_STATE.Walk));

                            yield return new WaitForSecondsRealtime(0.3f);  // ���������� ��� ���ð� �ο�
                            serchingTargetState = SerchingTargetState.Rotation;
                        }
                    }
                    else
                    {
                        walkTime += Time.deltaTime;
                        if (state == MONSTER_STATE.Idle)
                        {
                            state |= MONSTER_STATE.Walk;
                            animator.SetBool("Walk", state.HasFlag(MONSTER_STATE.Walk));
                        }

                        if (state.HasFlag(MONSTER_STATE.Walk))
                            transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);

                        if (randomAnimationCoolTime && walkTime > Random.Range(0.1f, 2.5f))
                        {

                            float random = Random.value;
                            if (random <= 0.1f)
                                yield return StartCoroutine(WaitForAnimation("Stretch", 1f));
                            else if (random <= 0.5f)
                                yield return StartCoroutine(WaitForAnimation("Look Around", 1f));
                            else
                                Debug.Log("�ൿ����");

                            randomAnimationCoolTime = false;                            // ����ǰ� ���� false�� �ٲ��ֱ�
                            StartCoroutine(CoolTime(Random.Range(10f, 20f), "RandomAnimation"));
                        }
                    }
                    break;
            }

            yield return null;
        }
    }
    void SetDestinationDirection(Transform targetPos, float angleLimit = 0f)    // ������ ������ ���� �ϴ� �Լ�
    {        
        // ���Ͱ� �󸶳� ȸ������ ���� ���ϱ�
        Vector3 dir = targetPos.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);   // ���� �ٶ� ����
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);   // �� ����� ��ǥ ������ ��������
        Debug.Log("Ÿ�ٰ� �������� : " + angleDifference + ", �������� : "+ angleLimit);
        if (angleDifference <= angleLimit)                          // angleLimit ������ ������ ���� �ʴ´�.
        {
            state &= ~MONSTER_STATE.Rotation;
            rotationCoroutine = null;
            return;
        }
        Debug.Log("���°� ���質?");
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
    IEnumerator Rotation(string name, float targetAngle)
    {
        
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))
            yield return null;

        float playTime = 0f;
        float exitTime = animator.GetCurrentAnimatorStateInfo(0).length;

        if (targetAngle < 60)
            exitTime = exitTime * 0.90f;    // ���� ����� ȸ�� �ִϸ��̼��� 90%�ð����� �ڷ�ƾ ����(ȸ���ϴ� ����� ������� �ʵ���)


        while (playTime < exitTime)
        {
            playTime += 0.02f;
            transform.Rotate(new Vector3(0, (targetAngle / (exitTime / 0.02f)), 0), Space.Self);
            
            yield return new WaitForFixedUpdate();
        }
        serchingTargetState = SerchingTargetState.Walk;
        state &= ~MONSTER_STATE.Rotation;
        animator.SetInteger("Rotation", 0);
    }

    bool CreateRandomDestination(Vector3 center, float range)
    {
        for (int i = 0; i < 40; i++)        // ������ �����ϴ� �������� ���ö����� ���� (�ִ� 40ȸ)
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
    bool startRoar = true;
    MONSTER_ATTACK_TYPE attackType;
    IEnumerator InBattle()
    {
        float targetDistance;
        float randomValue;

        InitState(behaviorState);                                       // ���� ���� �ʱ�ȭ

        Debug.Log("startRoar : " + startRoar);
        if (startRoar)
        {
            //yield return StartCoroutine(LookatTarget());              // Ÿ�� ó�ٺ���
            SetDestinationDirection(target.transform);
            yield return StartCoroutine(rotationCoroutine);
            yield return StartCoroutine(WaitForAnimation("Roar", 1f));  // ǥȿ�ϱ�
        }

        startRoar = true;

        while (true)
        {
            targetDistance = Vector3.Distance(transform.position, target.transform.position);

            if (attackCoolTime && (state == MONSTER_STATE.Idle || state == MONSTER_STATE.Walk))
            {
                if (targetDistance < 25)
                {
                    state &= ~MONSTER_STATE.Walk;
                    animator.SetBool("Walk", false);

                    state |= MONSTER_STATE.Rotation | MONSTER_STATE.Attack;
                    SetDestinationDirection(target.transform, 15);                  // Ÿ�ٹ���ã��(����, ���ͱ��� ��/��)
                    if(rotationCoroutine != null)
                        yield return StartCoroutine(rotationCoroutine);             // Ÿ�ٹ������� ȸ��
                    
                    randomValue = Random.value;
                    if (attackType == MONSTER_ATTACK_TYPE.Run)                  // Run�� �ι� ���� �ȳ�������
                    {
                        while(attackType == MONSTER_ATTACK_TYPE.Run)
                        {
                            attackType = DecideAttackType(targetDistance, randomValue); // �Ÿ��� ���� �������� ��������
                            yield return null;
                        }
                    }
                    else
                    {
                        attackType = DecideAttackType(targetDistance, randomValue);     // �Ÿ��� ���� �������� ��������
                    }
                }
                else                            // Ÿ�ٰ��� �Ÿ��� 25f �̻��϶� �ʹ� �־ �߰�
                {
                    state |= MONSTER_STATE.Walk;
                }
            }
            else if(!attackCoolTime)            // ��ų ��Ÿ���϶� �߰�
            {
                state |= MONSTER_STATE.Walk;
                // yield return StartCoroutine(RunToTartget());
            }

            if (state == MONSTER_STATE.Attack)  // ���ݽ���
            {
                switch (attackType)
                {
                    case MONSTER_ATTACK_TYPE.Stomp:
                        yield return StartCoroutine(WaitForAnimation("Stomp", 1f));
                        break;

                    case MONSTER_ATTACK_TYPE.Swipe:
                        yield return StartCoroutine(WaitForAnimation("Swipe", 1f));
                        break;

                    case MONSTER_ATTACK_TYPE.Roar:
                        yield return StartCoroutine(WaitForAnimation("Roar", 1f));
                        break;

                    case MONSTER_ATTACK_TYPE.JumpAttack:
                        yield return StartCoroutine(WaitForAnimation("JumpAttack", 1f));
                        break;

                    case MONSTER_ATTACK_TYPE.Throw:
                        yield return StartCoroutine(WaitForAnimation("Throw",1f));
                        break;

                    case MONSTER_ATTACK_TYPE.Run:
                        momentTargetPosition = target.transform.position;   // �޷��� ��ġ
                        yield return StartCoroutine(RunToTartget());
                        break;
                }
                state &= ~MONSTER_STATE.Attack;
                attackCoolTime = false;

                randomValue = Random.value;
                if (attackType == MONSTER_ATTACK_TYPE.Run)
                {
                    StartCoroutine(CoolTime(Random.Range(0.5f, 1.5f), "attackCoolTime"));
                    yield return new WaitForSecondsRealtime(Random.Range(0.2f, 0.6f));
                }
                else
                {
                    StartCoroutine(CoolTime(Random.Range(4f, 7f), "attackCoolTime"));
                    yield return new WaitForSecondsRealtime(Random.Range(0.2f, 0.6f));

                    if (randomValue < 0.3f)
                        yield return StartCoroutine(WaitForAnimation("Flexing Muscle", 1f));
                    else if (randomValue < 0.4f)
                        yield return StartCoroutine(WaitForAnimation("Stretch", 1f));
                    else
                        yield return new WaitForSecondsRealtime(Random.Range(0.5f, 1.1f));
                }
            }


            if(state.HasFlag(MONSTER_STATE.Walk))   // �ȱ� ���� ON�̸�
            {
                if (targetDistance > 7f)    // Ÿ�ٰ� �Ÿ��� 7f �ʰ��϶� �����ϱ�
                {
                    Debug.Log("�ȱ���� ���߰���");
                    ChaseTarget();
                }
                else                        // Ÿ�ٰ� �Ÿ��� 7f �����϶� �ȱ� X
                {
                    state &= ~MONSTER_STATE.Walk;
                    animator.SetBool("Walk", false);
                    attackCoolTime = true;  // ����� ������ ���� ����

                    //randomValue = Random.value;
                    //if (randomValue < 0.3f)
                    //    yield return StartCoroutine(WaitForAnimation("Flexing Muscle", 1f));
                    //else if (randomValue < 0.4f)
                    //    yield return StartCoroutine(WaitForAnimation("Stretch", 1f));
                }
            }

            yield return null;
        }
    }
    void InitState(MONSTER_BEHAVIOR_STATE behaviorState)                    
    {
        state = MONSTER_STATE.Idle;
        animator.SetInteger("Rotation", 0);
        animator.SetBool("Walk", state.HasFlag(MONSTER_STATE.Walk));
        
        if (behaviorState == MONSTER_BEHAVIOR_STATE.SerchingTarget)             // �ٲ���°� ��ġŸ�ٻ����϶�
        {
            animator.SetFloat("Breath Speed", 1f);
            serchingTargetState = SerchingTargetState.Rotation;
        }
        else if (behaviorState == MONSTER_BEHAVIOR_STATE.InBattle)              // �ٲ���°� �ι�ƲŸ�ٻ����϶�
        {
            
            attackCoolTime = false;
            animator.SetFloat("Breath Speed", 3f);
            StartCoroutine(CoolTime(Random.Range(2f, 4f), "attackCoolTime"));   // ���� ��Ÿ�� ����
            if (rotationCoroutine != null)
                StopCoroutine(rotationCoroutine);
        }
    }
    IEnumerator LookatTarget()
    {
        Vector3 dir = target.transform.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);   // ���� �ٶ� ����

        while (Quaternion.Angle(transform.rotation, targetRotation) > 4)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
            yield return null;
        }
    }
    IEnumerator RunToTartget()
    {
        animator.SetBool("Run", true);
        transform.LookAt(momentTargetPosition);
        Vector3 targetDir = (momentTargetPosition - transform.position).normalized;
        targetDir = new Vector3(targetDir.x, 0, targetDir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        
        while (Vector3.Distance(momentTargetPosition, transform.position) >= 7f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir.normalized), 2f * Time.deltaTime);
            transform.Translate(targetDir * 9 * Time.deltaTime, Space.World); // Ÿ�������� �̵�
            yield return null;
        }

        animator.SetBool("Run", false);
        yield return null;
    }
    void ChaseTarget()
    {
        animator.SetBool("Walk", true);
        transform.LookAt(target.transform);
        Vector3 targetDir = (target.transform.position - transform.position).normalized;
        targetDir = new Vector3(targetDir.x, 0, targetDir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir.normalized), 2f * Time.deltaTime);

        transform.Translate(targetDir * moveSpeed * Time.deltaTime, Space.World); // Ÿ�������� �̵�
    }

    MONSTER_ATTACK_TYPE DecideAttackType(float targetDistance, float randomValue)
    {
        if(attackType == MONSTER_ATTACK_TYPE.Run)
            randomValue += 0.2f;

        if (targetDistance < 6.5f)              // �Ÿ��� 6.5�̸� �϶� 
        {
            if (randomValue <= 0.7f)            // 70% Ȯ���� ������
                return MONSTER_ATTACK_TYPE.Stomp;
            else                                // 30% Ȯ���� ���ֵθ������
                return MONSTER_ATTACK_TYPE.Swipe;

        }
        else if (targetDistance < 8.5f)         // �Ÿ��� 8.5�̸� �϶� 
        {
            if (randomValue <= 0.65f)    
                return MONSTER_ATTACK_TYPE.Swipe;
            else
                return MONSTER_ATTACK_TYPE.JumpAttack;
        }
        else if (targetDistance < 16)           // �Ÿ��� 16�̸� �϶� 
        {
            if (randomValue <= 0.4f)
                return MONSTER_ATTACK_TYPE.JumpAttack;
            else if (randomValue <= 0.7f)
                return MONSTER_ATTACK_TYPE.Roar;
            else
                return MONSTER_ATTACK_TYPE.Run;
        }
        else                                    // �Ÿ��� 16~25 �϶� 
        {
            if (randomValue <= 0.3f)
                return MONSTER_ATTACK_TYPE.Roar;
            else if (randomValue <= 0.6f)
                return MONSTER_ATTACK_TYPE.Throw;
            else
                return MONSTER_ATTACK_TYPE.Run;
        }
    }

    float targetOutTime = 0;
    bool CheckTargetIsInArea()          
    {
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(target.transform.position, out hit, 2.0f, NavMesh.AllAreas))    // Ÿ���� ��������(�׺�޽�) ������ �������� 
        {
            targetOutTime += Time.deltaTime;

            if (targetOutTime > 3f)     // 3���̻� �ۿ� ������ Ÿ������
            {
                Debug.Log("Ÿ���� ���������� ���� �ð� : " + targetOutTime);
                target = null;
                return true;
            }
        }
        else    // ĳ���Ͱ� ���� ���� �ȿ� ���� ���
        {
            targetOutTime = 0;
        }
        return false;
    }
    
    #region �ִϸ��̼� �̺�Ʈ
    Projectile projectile;
    Vector3 momentTargetPosition;       // �� �ൿ�� �Ҷ� Ÿ���� ��ġ
    void CreateRock()           // �� ����
    {
        momentTargetPosition = target.transform.position;           // ���� ������ Ÿ���� ��ġ�� ���� ������ ����
        projectile = Instantiate(throwingObject[0], rightHand.position + rightHand.right, transform.rotation, rightHand).GetComponent<Rock>();
    }
    void ThrowRock()            // �� ������
    {
        projectile.Init(momentTargetPosition, 10, 25);              // �Ű����� (Ÿ���� ��ġ, ���ݷ�, �ӵ�)
    }
    void Roar()
    {
        if (state.HasFlag(MONSTER_STATE.Attack))
        {
            attackParticle.SetActive(true);
        }
    }
    void AttackStart(MONSTER_ATTACK_TYPE attackType)
    {
        switch(attackType)
        {
            case MONSTER_ATTACK_TYPE.Stomp:
                attackablePart[(int)attackType].Attack(true);
                break;
            case MONSTER_ATTACK_TYPE.Swipe:
                attackablePart[(int)attackType].Attack(true);
                break;
            case MONSTER_ATTACK_TYPE.JumpAttack:
                attackablePart[(int)attackType].Attack(true);
                break;
        }
        
    }
    void AttackEnd(MONSTER_ATTACK_TYPE attackType)
    {
        switch (attackType)
        {
            case MONSTER_ATTACK_TYPE.Stomp:
                attackablePart[(int)attackType].Attack(false);  // �޹�
                break;
            case MONSTER_ATTACK_TYPE.Swipe:
                attackablePart[(int)attackType].Attack(false);  // �޼�
                break;
            case MONSTER_ATTACK_TYPE.JumpAttack:
                attackablePart[(int)attackType].Attack(false);  // ������
                break;
        }

    }
    #endregion �ִϸ��̼� �̺�Ʈ

    #endregion InBattle 
    #region stagger
    public void StartStaggerState()
    {
        StopAllCoroutines();
        state |= MONSTER_STATE.Stagger;
        StartCoroutine(WaitForAnimation("Dying(Front Up)", 1f));
    }

    #endregion stagger
    public void ChangeState(MONSTER_BEHAVIOR_STATE newState)
    {
        StopCoroutine(behaviorState.ToString());                // ���� �������� �ڷ�ƾ ����
        behaviorState = newState;
        Debug.Log("ChangeState! �ٲ� ���� : " + newState);
        StartCoroutine(behaviorState.ToString());               // ����� ���·� �ڷ�ƾ ����
        
    }
    IEnumerator WaitForAnimation(string name, float exitRatio, int layer = -1)
    {
        float playTime =0;
        animator.Play(name, layer, 0);  // layer�� name�̸��� ���� �ִϸ��̼��� 0�ʺ��� �����ض�

        //animator.SetTrigger(name);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))   // �ִϸ��̼��� ��ȯ�ɶ����� ���
            yield return null;

        float exitTime = animator.GetCurrentAnimatorStateInfo(0).length * exitRatio;
        while (playTime < exitTime)
        {
            playTime += Time.deltaTime;
            yield return null;
        }

        if (name == "Dying(Front Up)")        // �������� 
        {
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                yield return null;
            }
            yield return new WaitForSecondsRealtime(0.1f);

            startRoar = false;
            ChangeState(MONSTER_BEHAVIOR_STATE.InBattle);
        }

        yield return null;

    }
    /*
        IEnumerator WaitForAnimation(string name, float exitRatio, bool isAttackAni = false, int layer = -1)
    {
        float playTime =0;
        animator.Play(name, layer, 0);  // layer�� name�̸��� ���� �ִϸ��̼��� 0�ʺ��� �����ض�

        //animator.SetTrigger(name);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))   // �ִϸ��̼��� ��ȯ�ɶ����� ���
            yield return null;

        float exitTime = animator.GetCurrentAnimatorStateInfo(0).length * exitRatio;
        while (playTime < exitTime)
        {
            playTime += Time.deltaTime;
            yield return null;
        }

        if (isAttackAni)   // "����"�ִϸ��̼��� ������ �������� 
        {
            attackCoolTime = false;
            StartCoroutine(CoolTime(Random.Range(4f, 7f), "attackCoolTime"));
        }

        if (name == "Dying(Front Up)")        // �������� 
        {
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                yield return null;
            }
            yield return new WaitForSecondsRealtime(0.1f);

            startRoar = false;
            ChangeState(MONSTER_BEHAVIOR_STATE.InBattle);
        }

        yield return null;

    }
     */
    private void OnDrawGizmos()
    {
        if (!DebugMode) return;
        Vector3 myPos = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawWireSphere(myPos, ViewRadius);

        float lookingAngle = transform.eulerAngles.y;  //ĳ���Ͱ� �ٶ󺸴� ������ ����
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        Vector3 lookDir = AngleToDir(lookingAngle);

        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, lookDir * ViewRadius, Color.green);
    }
}

