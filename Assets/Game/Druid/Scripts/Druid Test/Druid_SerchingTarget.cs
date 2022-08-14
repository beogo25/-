using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Druid_SerchingTarget : DruidAction
{

    [Header("�̵� ����"), Space(10)]
    [SerializeField] private Transform moveDestination;
    private float moveSpeed = 3;

    [Header("�þ߰� ����"), Space(10)]
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
    
    public override void Init()     // �� ���·� �ٲ������ ó���� ���۵�
    {
        Debug.Log("��ã���� Init");
        druidStatus.state = MONSTER_STATE.Rotation;
        animator.SetInteger("Rotation", 0);
        animator.SetBool("Walk", druidStatus.state.HasFlag(MONSTER_STATE.Walk));

        animator.SetFloat("Breath Speed", 1f);                      // Idle����(������)�� �ӵ��� ������� ���
        
        StartCoroutine(CoolTime(Random.Range(10f, 20f)));           // �����ִϸ��̼� ��Ÿ�ӵ�����
        StartCoroutine(druidStatus.behaviorState.ToString());       // ����̵� �ൿ����
    }
    void Update()
    {
        if (druidStatus.behaviorState != MONSTER_BEHAVIOR_STATE.SerchingTarget) return;     // SerchingTarget ���°� �ƴҰ�� update ����X

        if (FindTarget())
        {
            Debug.Log("��Ž���Ϸ�");
            ChangeState(MONSTER_BEHAVIOR_STATE.InBattle);
        }

    }
    #region Ÿ�� ����
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


    #endregion Ÿ�� ����
    //IEnumerator rotationCoroutine;
    IEnumerator SerchingTarget()
    {
        float walkTime = 0;

        while (true)
        {

            walkTime = 0;
            if (druidStatus.state == MONSTER_STATE.Rotation)            // Rotation���·� ������� (���°� �ٲ������ �ѹ��� ����ǵ���) 
            {
                SetDestinationDirection(moveDestination);               // ��ǥ���⿡ ���� ȸ���ִϸ��̼�

                yield return StartCoroutine(rotationCoroutine);
                druidStatus.state = MONSTER_STATE.Walk;
            }


            if (druidStatus.state == MONSTER_STATE.Walk)
            {
                if (Vector3.Distance(transform.position, moveDestination.position) < 8f)    // ��ǥ�� �Ÿ��� 8f �̳�
                {
                    monsterRigidbody.velocity = Vector3.zero;
                    if (CreateRandomDestination(moveDestination.position, 20f))
                    {
                        druidStatus.state &= ~MONSTER_STATE.Walk;
                        animator.SetBool("Walk", druidStatus.state.HasFlag(MONSTER_STATE.Walk));

                        yield return new WaitForSecondsRealtime(0.3f);  // ���������� ��� ���ð� �ο�
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
                            Debug.Log("�ൿ����");

                        randomAnimationCoolTime = false;                            // ����ǰ� ���� false�� �ٲ��ֱ�
                        StartCoroutine(CoolTime(Random.Range(10f, 20f)));
                    }
                }
            }

            yield return null;
        }
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


    IEnumerator WaitForAnimation(string name, float exitRatio, int layer = -1)
    {
        float playTime = 0;
        //animator.Play(name, layer, 0);  // layer�� name�̸��� ���� �ִϸ��̼��� 0�ʺ��� �����ض�
        animator.SetTrigger(name);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(name))   // �ִϸ��̼��� ��ȯ�ɶ����� ���
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

        float lookingAngle = transform.eulerAngles.y;  //ĳ���Ͱ� �ٶ󺸴� ������ ����
        Vector3 rightDir = AngleToDir(transform.eulerAngles.y + ViewAngle * 0.5f);
        Vector3 leftDir = AngleToDir(transform.eulerAngles.y - ViewAngle * 0.5f);
        Vector3 lookDir = AngleToDir(lookingAngle);

        Debug.DrawRay(myPos, rightDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, leftDir * ViewRadius, Color.blue);
        Debug.DrawRay(myPos, lookDir * ViewRadius, Color.green);
    }
}

/*
    void SetDestinationDirection(Transform targetPos, float angleLimit = 0f)    // ������ ������ ���� �ϴ� �Լ�
    {
        // ���Ͱ� �󸶳� ȸ������ ���� ���ϱ�
        Vector3 dir = targetPos.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);            // ���� �ٶ� ����
        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);   // �� rotation�� ��ǥ rotation�� ��������

        if (angleDifference <= angleLimit)                          // angleLimit ������ ������ ȸ������ �ʴ´�.
        {
            druidStatus.state &= ~MONSTER_STATE.Rotation;
            rotationCoroutine = null;
            return;
        }

        // ���Ͱ� � �������� ȸ������ ���ϱ� (������ or ����)
        Vector3 targetDir = targetPos.position - transform.position;                    // Ÿ�� �������� ���ϴ� ���͸� ���ϱ�
        Vector3 crossVec = Vector3.Cross(targetDir, this.transform.forward);            // foward�� ����
        float dot = Vector3.Dot(crossVec, Vector3.up);                                  // ������� ����
        if (dot > 0)        // ����
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
        else if (dot < 0)   // ������
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
 */