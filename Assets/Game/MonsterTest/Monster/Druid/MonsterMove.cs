using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterBehaviorState
{
    SerchingTarget,
    ChasingTarget,
    InBattle
}

public class MonsterMove : MonoBehaviour
{
    private enum State
    {
        Idle = 1 << 0,
        Walk = 1 << 1,
        Rotation = 1 << 2,

        All = Walk | Rotation,
    }
    private MonsterBehaviorState behaviorState;
    private State state = 0;

    private Collider target;
    private Animator animator;
    private Rigidbody monsterRigidbody;
    
    [SerializeField] private Transform moveDestination;
    [SerializeField] private float moveSpeed = 2;
    
    private bool randomAnimationStart = false;

    [SerializeField] private bool DebugMode = true;
    [Range(0f, 360f)]
    [SerializeField] private float ViewAngle = 0f;
    [SerializeField] private float ViewRadius = 1f;
    [SerializeField] private LayerMask TargetMask;
    [SerializeField] private LayerMask ObstacleMask;

    void Start()
    {
        animator = GetComponent<Animator>();
        monsterRigidbody = GetComponent<Rigidbody>();
        
        StartCoroutine(RandomAnimation(Random.Range(10f, 20f)));
        behaviorState = MonsterBehaviorState.SerchingTarget;
        StartCoroutine(behaviorState.ToString());
    }
    private void Update()
    {
        if (behaviorState == MonsterBehaviorState.SerchingTarget && target == null)
        {
            if (FindTarget())
                ChangeState(MonsterBehaviorState.ChasingTarget);
        }
    }

    #region SerchingTargetState
    private enum SerchingTargetState { Rotation, Walk };
    SerchingTargetState serchingTargetState = SerchingTargetState.Rotation;
    Quaternion targetRotation;
    IEnumerator SerchingTarget()
    {
        while (true)
        {
            switch (serchingTargetState)
            {
                case SerchingTargetState.Rotation:
                    if (!state.HasFlag(State.Rotation))         // Rotation���·� ������� (���°� �ٲ������ �ѹ��� ����ǵ���) 
                    {
                        state |= State.Rotation;
                        targetRotation = SetDestinationDirection();
                    }

                    if (Quaternion.Angle(transform.rotation, targetRotation) < 4)   // �� ���� ���̰� ����(4��) �����϶� ��ǥ�� ���� �ִ°����� �Ǵ�
                    {
                        serchingTargetState = SerchingTargetState.Walk;
                        state &= ~State.Rotation;
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
                        if (!state.HasFlag(State.Walk))
                        {
                            state |= State.Walk;
                            animator.SetBool("Walk", state.HasFlag(State.Walk));
                        }

                        if (state.HasFlag(State.Walk))
                        {
                            transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
                        }

                        if (randomAnimationStart)
                        {
                            int random = Random.Range(0, 2);
                            if (random == 0)
                                yield return StartCoroutine(WaitForAnimation("Stretch", 1f));
                            else if (random == 1)
                                yield return StartCoroutine(WaitForAnimation("Look Around", 1f));
                            else
                                Debug.Log("�ൿ����");

                            randomAnimationStart = false;                            // ����ǰ� ���� false�� �ٲ��ֱ�
                            StartCoroutine(RandomAnimation(Random.Range(10f, 20f)));
                        }

                    }
                    break;

                default:
                    break;
            }

            yield return null;
        }
    }
    Quaternion SetDestinationDirection()                            // ������ ������ ���� �ϴ� �Լ�
    {        
        // ���Ͱ� �󸶳� ȸ������ ���� ���ϱ�
        Vector3 dir = moveDestination.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        targetRotation = Quaternion.LookRotation(dir.normalized);   // ���� �ٶ� ����
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);   // �� ����� ��ǥ ������ ��������

        // ���Ͱ� � �������� ȸ������ ���ϱ� 
        Vector3 targetDir = moveDestination.position - transform.position;              // Ÿ�� �������� ���ϴ� ���͸� ���ϱ�
        Vector3 crossVec = Vector3.Cross(targetDir, this.transform.forward);            // foward�� ����
        float dot = Vector3.Dot(crossVec, Vector3.up);                                  // ������� ����
        if (dot > 0) // ����
        {
            if (angleDifference > 60)
            {
                animator.SetInteger("Rotation", -2);
                StartCoroutine(Rotation("Turn Left", -angleDifference));
            }
            else
            {
                animator.SetInteger("Rotation", -1);
                StartCoroutine(Rotation("Turn Left Slow", -angleDifference));
            }
        }
        else if (dot < 0) // ������
        {
            if (angleDifference > 60)
            {
                animator.SetInteger("Rotation", 2);
                StartCoroutine(Rotation("Turn Right", angleDifference));
            }
            else
            {
                animator.SetInteger("Rotation", 1);
                StartCoroutine(Rotation("Turn Right Slow",angleDifference));
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
    }
    /*
        IEnumerator Rotation(float targetAngle)
    {
        yield return new WaitForSecondsRealtime(0.05f);  // �ִϸ��̼� ��ȯ�ð�����
        float playTime = 0f;
        float time = animator.GetCurrentAnimatorStateInfo(0).length;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            Debug.Log("Idle�� : " + time);
            time = 0.93f - 0.30f;
        }
        else
        {
            Debug.Log("�ߵ� : " + time);
            time = time * 0.60f;                        // ���� ����� ȸ�� �ִϸ��̼��� 60%�ð����� �ڷ�ƾ ����(ȸ���ϴ� ����� ������� �ʵ���)
        }

        for (int i = 0; i < 50; i++)                    // 50ȸ�� ���� ȸ���ϱ�
        {
            playTime += Time.deltaTime;
            Debug.Log("�ɸ��ð� : " + playTime);
            transform.Rotate(new Vector3(0, (targetAngle / 50f), 0), Space.Self);
            yield return new WaitForSecondsRealtime(time / 50f);
        }
    }
     */
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
    IEnumerator RandomAnimation(float cool)
    {
        while (cool > 0)
        {
            cool -= Time.deltaTime;
            yield return null;
        }
        randomAnimationStart = true;
    }
    #endregion SerchingTargetState

    #region ChasingTartget
    IEnumerator ChasingTarget()
    {
        // ���� ���� �ʱ�ȭ
        InitState();

        // ���� �߽߰� Roar ����
        //Vector3 dir = moveDestination.position - transform.position;
        //dir = new Vector3(dir.x, 0, dir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        //targetRotation = Quaternion.LookRotation(dir.normalized);   // ���� �ٶ� ����
        transform.LookAt(target.transform.position);
        yield return StartCoroutine(WaitForAnimation("Roar",1f));

        NavMeshHit hit;
        while (true)
        {
            if (!NavMesh.SamplePosition(target.transform.position, out hit, 2.0f, NavMesh.AllAreas))        // Ÿ���� �������� ������ �������� 
            {
                target = null;
                yield return new WaitForSecondsRealtime(0.1f);
                ChangeState(MonsterBehaviorState.SerchingTarget);
            }
            //Debug.Log("������ :" + behaviorState.ToString());
            yield return null;
        }
    }
    void InitState()
    {
        state = State.Idle;
        animator.SetInteger("Rotation", 0);
        animator.SetBool("Walk", state.HasFlag(State.Walk));
    }

    #endregion ChasingTartget
    public void ChangeState(MonsterBehaviorState newState)
    {
        StopCoroutine(behaviorState.ToString());              // ���� �������� �ڷ�ƾ ����
        behaviorState = newState;
        Debug.Log("ChangeState! �ٲ� ���� : " + newState);
        StartCoroutine(behaviorState.ToString());             // ����� ���·� �ڷ�ƾ ����
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
            yield return new WaitForEndOfFrame();
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

