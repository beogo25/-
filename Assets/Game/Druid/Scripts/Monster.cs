using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MONSTER_BEHAVIOR_STATE
{
    SerchingTarget,
    InBattle,
}

public class Monster : MonoBehaviour
{
    private enum State
    {
        Idle = 0,
        Walk = 1 << 0,
        Rotation = 1 << 1,
        Attack = 1 << 2,
        //Idle = 1 << 0,
        //Walk = 1 << 1,
        //Rotation = 1 << 2,
        //Attack = 1 << 3,

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
    
    [Header("�þ߰� ����"),Space(10)]
    [SerializeField] private bool DebugMode = true;
    [Range(0f, 360f)]
    [SerializeField] private float ViewAngle = 0f;
    [SerializeField] private float ViewRadius = 1f;
    [SerializeField] private LayerMask TargetMask;
    [SerializeField] private LayerMask ObstacleMask;

    [Header("��������"), Space(10)]
    [SerializeField] private GameObject[] throwingObject;
    [SerializeField] private Collider[] bodyCollider;
    [SerializeField] private Transform rightHand;
    void Start()
    {
        animator = GetComponent<Animator>();
        monsterRigidbody = GetComponent<Rigidbody>();
        
        StartCoroutine(CoolTime(Random.Range(10f, 20f),"RandomAnimation"));
        behaviorState = MONSTER_BEHAVIOR_STATE.SerchingTarget;
        //StartCoroutine(behaviorState.ToString());
    }
    private void Update()
    {
        //if (behaviorState == MONSTER_BEHAVIOR_STATE.SerchingTarget && target == null)
        //{
        //    if (FindTarget())
        //    {
        //        ChangeState(MONSTER_BEHAVIOR_STATE.InBattle);
        //    }
        //}
        //else if (behaviorState == MONSTER_BEHAVIOR_STATE.InBattle && target != null)
        //{
        //    if (CheckTargetIsInArea())
        //        ChangeState(MONSTER_BEHAVIOR_STATE.SerchingTarget);
        //}



        // �׽�Ʈ �κ�


        //if (target != null)
        //{
        //    //transform.LookAt(target.transform);
        //    Vector3 targetD = (target.transform.position - transform.position).normalized;
        //    targetD = new Vector3(targetD.x, 0, targetD.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
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
                Debug.Log("Ÿ�پ���");
            }
            else {
                Debug.Log("Ÿ�� : " + target.name);
                //SetDestinationDirection(target.transform);
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("���� state : " + state);
            animator.SetTrigger("Jump Attack");
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("���� state : " + state);
            animator.SetTrigger("Swipe");
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
                    if (state == State.Idle)         // Rotation���·� ������� (���°� �ٲ������ �ѹ��� ����ǵ���) 
                    {
                        state |= State.Rotation;
                        targetRotation = SetDestinationDirection(moveDestination);
                    }
                    
                    if (Quaternion.Angle(transform.rotation, targetRotation) < 4)   // �� ���� ���̰� ����(4��) �����϶� ��ǥ�� ���� �ִ°����� �Ǵ�
                    {
                        serchingTargetState = SerchingTargetState.Walk;
                        //animator.SetInteger("Rotation", 0);
                    }

                    break;

                case SerchingTargetState.Walk:
                    if (Vector3.Distance(transform.position, moveDestination.position) < 4f)    // ��ǥ�� �Ÿ��� 4f �̳�
                    {
                        monsterRigidbody.velocity = Vector3.zero;
                        if (CreateRandomDestination(moveDestination.position, 20f))
                        {
                            state &= ~State.Walk;
                            animator.SetBool("Walk", state.HasFlag(State.Walk));

                            yield return new WaitForSecondsRealtime(Random.Range(0.3f, 1.3f));  // ���������� ��� ���ð� �ο�
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
    Quaternion SetDestinationDirection(Transform targetPos, float angleLimit = 360f)                            // ������ ������ ���� �ϴ� �Լ�
    {        
        // ���Ͱ� �󸶳� ȸ������ ���� ���ϱ�
        Vector3 dir = targetPos.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        targetRotation = Quaternion.LookRotation(dir.normalized);   // ���� �ٶ� ����
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);   // �� ����� ��ǥ ������ ��������
        Debug.Log("�������� : " + angleDifference);
        if (angleDifference >= angleLimit)
        {
            state &= ~State.Rotation;
            return targetRotation;
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
                StartCoroutine(rotationCoroutine);
            }
            else
            {
                animator.SetInteger("Rotation", -1);
                rotationCoroutine = Rotation("Turn Left Slow", -angleDifference);
                StartCoroutine(rotationCoroutine);
            }
        }
        else if (dot < 0) // ������
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
        else // ��� (0�϶�)
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
        exitTime = exitTime * 0.30f;                        // ���� ����� ȸ�� �ִϸ��̼��� 30%�ð����� �ڷ�ƾ ����(ȸ���ϴ� ����� ������� �ʵ���)
        
        while (playTime < exitTime)
        {
            playTime += 0.01f;
            transform.Rotate(new Vector3(0, (targetAngle / (exitTime / 0.01f)), 0), Space.Self);
            yield return new WaitForSecondsRealtime(0.01f); ;
        }
        state &= ~State.Rotation;
        animator.SetInteger("Rotation", 0);
        //Debug.Log("�����̼� ������ �� : " + state);
    }

    bool CreateRandomDestination(Vector3 center, float range)
    {
        for (int i = 0; i < 40; i++)        // ������ �����ϴ� �������� ���ö����� ���� (�ִ� 40ȸ)
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
        Collider[] targets = Physics.OverlapSphere(transform.position, ViewRadius, TargetMask);

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
        Debug.Log("��Ÿ�ӵ���");
        while (cool > 0)
        {
            cool -= Time.deltaTime;
            yield return null;
            //if(func == "attackCoolTime")
            //    Debug.Log(cool);
        }
        switch(func)
        {
            case "RandomAnimation":
                randomAnimationCoolTime = true;
                break;
            case "attackCoolTime":
                Debug.Log("���� ��Ÿ�� ON");
                attackCoolTime = true;
                break;
        }
        
    }
    #endregion SerchingTargetState


    #region InBattle
    enum InBattleState { Rotation, Walk, Attack};
    InBattleState inBattleState = InBattleState.Rotation;
    bool attackCoolTime = false;
    int attackType;
    IEnumerator InBattle()
    {
        float targetDistance;
        
        // ���� ���� �ʱ�ȭ
        InitState();
        
        yield return StartCoroutine(LookatTarget());                // Ÿ�� ó�ٺ���
        yield return StartCoroutine(WaitForAnimation("Roar",1f));   // ǥȿ�ϱ�

        
        while (true)
        {
            //Debug.Log("���� ��Ÿ�� : " + attackCoolTime + ", state : " + state);
            targetDistance = Vector3.Distance(transform.position, target.transform.position);
            if (attackCoolTime && (state == State.Idle))
            {
                //Debug.Log("Ÿ�ٰ��� �Ÿ� : " + targetDistance);
                if (targetDistance < 6)     // ��� ����
                {
                    animator.SetBool("Walk", false);
                    Debug.Log("��� ������ ���� ȸ�� : " + targetDistance);
                    attackType = 0;
                    state |= State.Rotation | State.Attack;
                    SetDestinationDirection(target.transform);
                }
                else if (15 < targetDistance && targetDistance < 25) // ���Ÿ� ����
                {
                    animator.SetBool("Walk", false);
                    Debug.Log("���Ÿ� ������ ���� ȸ�� : " + targetDistance);
                    attackType = 1;
                    state |= State.Rotation | State.Attack;
                    SetDestinationDirection(target.transform, 30);
                }
                else        // �ʹ��� �־ �߰�
                {
                    ChaseTarget();
                }

            }

            if (state == State.Attack) // 
            {
                //Debug.Log("��� ���� ���� ���� ���� : " + state);
                switch (attackType)
                {
                    case 0:
                        yield return StartCoroutine(WaitForAnimation("Stomp", 1f,true));
                        break;

                    case 1:
                        rightHand.gameObject.GetComponent<CapsuleCollider>().enabled = false;   // �ݴ� ��ǽ� �ݶ��̴����� ĳ���Ͱ� �ضߴ°� ����
                        momentTargetPosition = target.transform.position;   // ���� �ݴ� ���۽� Ÿ�� ��ġ 
                        yield return StartCoroutine(WaitForAnimation("Throw",1f,true));
                        break;
                }
                
                state &= ~State.Attack;
                Debug.Log("���� ��, ���� ���� : " + attackCoolTime);
                
            }

            if (!attackCoolTime)
            {
                if(targetDistance > 10f)
                    ChaseTarget();
                else
                    animator.SetBool("Walk", false);
            }


            // �Ÿ��� ���� ����?
            // ���ʸ��� ����?

            yield return null;
        }
    }
    void InitState()
    {
        attackCoolTime = false;

        state = State.Idle;                                 
        animator.SetInteger("Rotation", 0);
        animator.SetBool("Walk", state.HasFlag(State.Walk));

        StartCoroutine(CoolTime(Random.Range(3f, 6f), "attackCoolTime"));   // ���� ��Ÿ�� ����
        if (rotationCoroutine!=null)
            StopCoroutine(rotationCoroutine);
    }
    IEnumerator LookatTarget()
    {
        Vector3 dir = target.transform.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        targetRotation = Quaternion.LookRotation(dir.normalized);   // ���� �ٶ� ����

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
        targetD = new Vector3(targetD.x, 0, targetD.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetD.normalized), 2f * Time.deltaTime);

        transform.Translate(targetD * moveSpeed * Time.deltaTime, Space.World);

    }
    float targetOutTime = 0;
    bool CheckTargetIsInArea()          
    {
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(target.transform.position, out hit, 2.0f, NavMesh.AllAreas)) // ĳ���Ͱ� ���������� �ƴ϶��       // Ÿ���� �������� ������ �������� 
        {
            targetOutTime += Time.deltaTime;

            if (targetOutTime > 3f)     // 3���̻� �ۿ� ������ Ÿ������
            {
                target = null;
                Debug.Log("Ÿ���� NULL");
                return true;
            }
        }
        else    // ĳ���Ͱ� ���� ���� �ȿ� ���� ���
        {
            targetOutTime = 0;
        }
        
        return false;
    }
    Projectile projectile;
    Vector3 momentTargetPosition;
    void RangeAttack(int value) // �ִϸ��̼� �̺�Ʈ (���������϶��� �ߵ�)
    {
        
        
        switch (value)
        {
            case 0:     // ��������
                projectile = Instantiate(throwingObject[0], rightHand.position + rightHand.right, transform.rotation, rightHand).GetComponent<Rock>();
                
                Debug.Log("�����ӽ���");
                break;

            case 1:     // �����ֵθ���
                projectile = Instantiate(throwingObject[1], rightHand.position - rightHand.up*1.5f, transform.rotation, rightHand).GetComponent<Wood>();
                animator.SetTrigger("Swing");

                break;
        }
    }
    void ThrowProjectile()     // �ִϸ��̼� �̺�Ʈ (���������϶��� �ߵ�)
    {
        if (projectile is Rock)
        {
            projectile.Init(momentTargetPosition, 10, 25);   // �Ű����� (Ÿ���� ��ġ, ���ݷ�, �ӵ�)
            rightHand.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
        else if(projectile is Wood)
        {
            projectile.Init((transform.right*2 - transform.up), 15, 50);   // �Ű����� (Ÿ���� ��ġ, ���ݷ�, �ӵ�)
            rightHand.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }

    }

    #endregion InBattle
    public void ChangeState(MONSTER_BEHAVIOR_STATE newState)
    {
        StopCoroutine(behaviorState.ToString());                // ���� �������� �ڷ�ƾ ����
        behaviorState = newState;
        Debug.Log("ChangeState! �ٲ� ���� : " + newState);
        StartCoroutine(behaviorState.ToString());               // ����� ���·� �ڷ�ƾ ����
    }
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

        if (isAttackAni)   // ���ݾִϸ��̼��� ������ �������� 
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

        float lookingAngle = transform.eulerAngles.y;  //ĳ���Ͱ� �ٶ󺸴� ������ ����
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        Vector3 lookDir = AngleToDir(lookingAngle);

        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, lookDir * ViewRadius, Color.green);
    }
}

