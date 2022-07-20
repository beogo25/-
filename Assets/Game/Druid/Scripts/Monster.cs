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
        Idle = 1 << 0,
        Walk = 1 << 1,
        Rotation = 1 << 2,
        Attack = 1 << 3,

        All = Walk | Rotation | Attack,
    }
    private MONSTER_BEHAVIOR_STATE behaviorState;
    private State state = State.Idle;

    private Collider target;
    private Animator animator;
    private Rigidbody monsterRigidbody;
    
    [SerializeField] private Transform moveDestination;
    private bool RandomAnimationCoolTime = false;
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
    [SerializeField] private Transform rightHand;
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
            if (!CheckTargetIsInArea())
                ChangeState(MONSTER_BEHAVIOR_STATE.SerchingTarget);
        }

        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    StartCoroutine(RangeAttack("Rock"));
        //}
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    StartCoroutine(RangeAttack("Wood"));
        //}

        if (Input.GetKeyDown(KeyCode.I))
        {
            animator.SetTrigger("Stomp");
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
            Debug.Log(state);
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
                        animator.SetInteger("Rotation", 0);
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

                        if (RandomAnimationCoolTime)
                        {
                            int random = Random.Range(0, 2);
                            if (random == 0)
                                yield return StartCoroutine(WaitForAnimation("Stretch", 1f));
                            else if (random == 1)
                                yield return StartCoroutine(WaitForAnimation("Look Around", 1f));
                            else
                                Debug.Log("�ൿ����");

                            RandomAnimationCoolTime = false;                            // ����ǰ� ���� false�� �ٲ��ֱ�
                            StartCoroutine(CoolTime(Random.Range(10f, 20f), "RandomAnimation"));
                            
                        }
                    }
                    break;
            }

            yield return null;
        }
    }
    Quaternion SetDestinationDirection(Transform targetPos)                            // ������ ������ ���� �ϴ� �Լ�
    {        
        // ���Ͱ� �󸶳� ȸ������ ���� ���ϱ�
        Vector3 dir = targetPos.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        targetRotation = Quaternion.LookRotation(dir.normalized);   // ���� �ٶ� ����
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);   // �� ����� ��ǥ ������ ��������

        // ���Ͱ� � �������� ȸ������ ���ϱ� 
        Vector3 targetDir = targetPos.position - transform.position;              // Ÿ�� �������� ���ϴ� ���͸� ���ϱ�
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
    WaitForSecondsRealtime rotationDelay = new WaitForSecondsRealtime(0.01f);
    IEnumerator Rotation(string name, float targetAngle)
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))
            yield return null;

        float playTime = 0f;
        float exitTime = animator.GetCurrentAnimatorStateInfo(0).length;
        
        exitTime = exitTime * 0.30f;                        // ���� ����� ȸ�� �ִϸ��̼��� 30%�ð����� �ڷ�ƾ ����(ȸ���ϴ� ����� ������� �ʵ���)
        
        while (playTime < exitTime)
        {
            playTime += 0.01f;
            transform.Rotate(new Vector3(0, (targetAngle / (exitTime / 0.01f)), 0), Space.Self);
            yield return rotationDelay;
        }
        state &= ~State.Rotation;
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
        while (cool > 0)
        {
            cool -= Time.deltaTime;
            yield return null;
        }
        switch(func)
        {
            case "RandomAnimation":
                RandomAnimationCoolTime = true;
                break;
        }
        
    }
    #endregion SerchingTargetState
    

    #region InBattle
    IEnumerator InBattleCoroutine;
    float targetDistance;
    bool attackCoolTime = false;
    IEnumerator InBattle()
    {
        // ���� ���� �ʱ�ȭ
        InitState();

        yield return StartCoroutine(LookatTarget());                // Ÿ�� ó�ٺ���
        yield return StartCoroutine(WaitForAnimation("Roar",1f));   // ǥȿ�ϱ�

        while (true)
        {
            targetDistance = Vector3.Distance(transform.position, target.transform.position);
            if (attackCoolTime && state == State.Idle)
            {
                if (targetDistance < 6)     // �ٰŸ�����
                {
                    //state |= State.Rotation;
                    targetRotation = SetDestinationDirection(target.transform);

                }
                else if (targetDistance < 25) // ���Ÿ� ����
                {
                    
                }
                else    // �ʹ� �־ �����ϱ�
                {
                    
                }
            }
                
            Debug.Log("�Ÿ����̴� : " + targetDistance);
            // �Ÿ��� ���� ����?
            // ���ʸ��� ����?

            yield return null;
        }
    }
    void InitState()
    {
        state = State.Idle;
        animator.SetInteger("Rotation", 0);
        animator.SetBool("Walk", state.HasFlag(State.Walk));
        if(rotationCoroutine!=null)
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
    bool CheckTargetIsInArea()          // Ÿ���� ������������ ������ target�� Null�� ��������
    {
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(target.transform.position, out hit, 2.0f, NavMesh.AllAreas))        // Ÿ���� �������� ������ �������� 
        {
            target = null;
            return false;
        }
        return true;
    }
    Projectile projectile;
    IEnumerator RangeAttack(int value)
    {
        rightHand.gameObject.GetComponent<CapsuleCollider>().enabled = false;   // �ݴ� ��ǽ� �ݶ��̴����� ĳ���Ͱ� �ضߴ°� ����
        animator.SetTrigger("Throw");
        yield return new WaitForSeconds(0.8f);

        
        switch (value)
        {
            case 0:     // ��������
                projectile = Instantiate(throwingObject[0], rightHand.position + rightHand.right, transform.rotation, rightHand).GetComponent<Rock>();                
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
            projectile.Init(target.transform.position, 10, 25);   // �Ű����� (Ÿ���� ��ġ, ���ݷ�, �ӵ�)
            rightHand.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
        else if(projectile is Wood)
        {
            projectile.Init((transform.right*2 - transform.up), 15, 50);   // �Ű����� (Ÿ���� ��ġ, ���ݷ�, �ӵ�)
            rightHand.gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
    }
    IEnumerator Swing(Projectile gameObject)
    {
        animator.SetTrigger("Swing");
        yield return new WaitForSeconds(3.80f);
        gameObject.transform.parent = null;


    }
    #endregion InBattle
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

