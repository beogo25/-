using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Druid_InBattle : DruidAction
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
    [Header("���� ����"), Space(10)]
    [SerializeField] private GameObject throwingObject;
    //[SerializeField] private MonsterHitablePart[] hitablePart;        // �ǰݺ��� ����
    [SerializeField] private DruidAttackablePart[] attackablePart;      // ���ݺ��� ����
    [SerializeField] private Transform rightHand;
    [SerializeField] private GameObject attackParticle;

    float moveSpeed = 3;
    bool attackCoolTime = false;
    bool startRoar = true;
    MONSTER_ATTACK_TYPE attackType;
    private new void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        if (druidStatus.behaviorState != MONSTER_BEHAVIOR_STATE.InBattle) return;     // InBattle ���°� �ƴҰ�� update ����X

        if (CheckTargetIsInArea())
            ChangeState(MONSTER_BEHAVIOR_STATE.SerchingTarget);

        if(Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("state : " + druidStatus.state + ", ani state : " + animator.GetBool("Dead"));
        }
    }

    float targetOutTime = 0;
    bool CheckTargetIsInArea()
    {
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(druidStatus.target.transform.position, out hit, 2.0f, NavMesh.AllAreas))    // Ÿ���� ��������(�׺�޽�) ������ �������� 
        {
            targetOutTime += Time.deltaTime;

            if (targetOutTime > 3f)     // 3���̻� �ۿ� ������ Ÿ������
            {
                Debug.Log("Ÿ���� ���������� ���� �ð� : " + targetOutTime);
                druidStatus.target = null;
                return true;
            }
        }
        else    // ĳ���Ͱ� ���� ���� �ȿ� ���� ���
        {
            targetOutTime = 0;
        }
        return false;
    }
    public override void Init()
    {
        Debug.Log("������ Init");
        druidStatus.state = MONSTER_STATE.Idle;
        animator.SetInteger("Rotation", 0);
        animator.SetBool("Walk", druidStatus.state.HasFlag(MONSTER_STATE.Walk));

        attackCoolTime = false;
        animator.SetFloat("Breath Speed", 3f);
        StartCoroutine(CoolTime(Random.Range(2f, 4f)));   // ���� ��Ÿ�� ����

        druidStatus.PlaybattleBGM();
       
        
        StartCoroutine(druidStatus.behaviorState.ToString());
    }

    IEnumerator InBattle()
    {
        Debug.Log("InBattle ����");
        float targetDistance;
        float randomValue;

        if (startRoar)                                                  // ù �������� ����
        {
            SetDestinationDirection(druidStatus.target.transform);
            yield return StartCoroutine(rotationCoroutine);
            yield return StartCoroutine(WaitForAnimation("Roar", 1f));  // ǥȿ�ϱ�
        }

        startRoar = true;

        while (true)
        {
            targetDistance = Vector3.Distance(transform.position, druidStatus.target.transform.position);

            if (attackCoolTime && (druidStatus.state == MONSTER_STATE.Idle || druidStatus.state == MONSTER_STATE.Walk))
            {
                if (targetDistance < 30)
                {
                    druidStatus.state &= ~MONSTER_STATE.Walk;
                    animator.SetBool("Walk", false);

                    druidStatus.state |= MONSTER_STATE.Rotation | MONSTER_STATE.Attack;
                    SetDestinationDirection(druidStatus.target.transform, 18);      // Ÿ�ٹ���ã��(����, ���ͱ��� ��/��)
                    if (rotationCoroutine != null)
                        yield return StartCoroutine(rotationCoroutine);             // Ÿ�ٹ������� ȸ��

                    randomValue = Random.value;
                    attackType = DecideAttackType(targetDistance, randomValue);     // �Ÿ��� ���� �������� ��������
                }
                else                                // Ÿ�ٰ��� �Ÿ��� 30f �̻��϶� �ʹ� �־ �߰�
                {
                    if(!druidStatus.state.HasFlag(MONSTER_STATE.Walk))
                        druidStatus.state |= MONSTER_STATE.Walk;
                }
            }
            else if (!attackCoolTime)               // ��ų ��Ÿ���϶� �߰�
            {
                if (!druidStatus.state.HasFlag(MONSTER_STATE.Walk))
                    druidStatus.state |= MONSTER_STATE.Walk;
                // yield return StartCoroutine(RunToTartget());
            }


            if (druidStatus.state == MONSTER_STATE.Attack)  // ���ݽ���
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
                        yield return StartCoroutine(WaitForAnimation("Throw", 1f));
                        break;

                    case MONSTER_ATTACK_TYPE.Run:
                        
                        yield return StartCoroutine(RunToTartget());
                        break;
                }
                attackCoolTime = false;
                druidStatus.state &= ~MONSTER_STATE.Attack;

                //Debug.Log(attackType + " �� ���� : " + druidStatus.state);

                randomValue = Random.value;
                if (attackType == MONSTER_ATTACK_TYPE.Run)
                {
                    StartCoroutine(CoolTime(Random.Range(0.5f, 1f)));
                    yield return new WaitForSecondsRealtime(Random.Range(0.2f, 0.6f));
                }
                else
                {
                    StartCoroutine(CoolTime(Random.Range(4f, 8f)));
                    yield return new WaitForSecondsRealtime(0.2f);

                    if (randomValue < 0.3f)
                        yield return StartCoroutine(WaitForAnimation("Flexing Muscle", 1f));
                    else if (randomValue < 0.4f)
                        yield return StartCoroutine(WaitForAnimation("Stretch", 1f));
                    //else if(randomValue < 0.8f)
                    //{
                    //    SetDestinationDirection(druidStatus.target.transform);
                    //    if (rotationCoroutine != null)
                    //        yield return StartCoroutine(rotationCoroutine);
                    //}
                    else
                        yield return new WaitForSecondsRealtime(Random.Range(0.3f, 0.7f));
                }
            }
   

            if (druidStatus.state.HasFlag(MONSTER_STATE.Walk))   // �ȱ� ���� ON�̸�
            {
                if (targetDistance > 7f)    // Ÿ�ٰ� �Ÿ��� 7f �ʰ��϶� �����ϱ�
                {
                    ChaseTarget();
                }
                else                        // Ÿ�ٰ� �Ÿ��� 7f �����϶� �ȱ� X
                {
                    druidStatus.state &= ~MONSTER_STATE.Walk;
                    animator.SetBool("Walk", false);
                    attackCoolTime = true;  // ����� ������ ���ݽ���
                }
            }

            yield return null;
        }
    }
    MONSTER_ATTACK_TYPE DecideAttackType(float targetDistance, float randomValue)
    {
        if (attackType == MONSTER_ATTACK_TYPE.Run)
            randomValue -= 0.2f;

        if (targetDistance < 6.5f)              // �Ÿ��� 6.5�̸� �϶� 
        {
            if (randomValue <= 0.6f)            // 60% Ȯ���� ������
                return MONSTER_ATTACK_TYPE.Stomp;
            else if(randomValue <= 0.8f)        // 20% Ȯ���� ���ֵθ������
                return MONSTER_ATTACK_TYPE.Swipe;
            else 
                return MONSTER_ATTACK_TYPE.JumpAttack;

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
            if (randomValue <= 0.3f)
                return MONSTER_ATTACK_TYPE.JumpAttack;
            else if (randomValue <= 0.55f)
                return MONSTER_ATTACK_TYPE.Roar;
            else if (randomValue <= 0.8f)
                return MONSTER_ATTACK_TYPE.Throw;
            else
                return MONSTER_ATTACK_TYPE.Run;
        }
        else                                    // �Ÿ��� 16~30�϶� 
        {
            if (randomValue <= 0.25f)
                return MONSTER_ATTACK_TYPE.Roar;
            else if (randomValue <= 0.65f)
                return MONSTER_ATTACK_TYPE.Throw;
            else
                return MONSTER_ATTACK_TYPE.Run;
        }
    }
    void ChaseTarget()
    {
        animator.SetBool("Walk", true);
        //transform.LookAt(target.transform); // ������ ����ұ� ? 
        Vector3 targetDir = (druidStatus.target.transform.position - transform.position).normalized;
        targetDir = new Vector3(targetDir.x, 0, targetDir.z);                           // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir.normalized), 3f * Time.deltaTime);

        transform.Translate(targetDir * moveSpeed * Time.deltaTime, Space.World);       // Ÿ�������� �̵�
    }
    IEnumerator RunToTartget()
    {
        momentTargetPosition = druidStatus.target.transform.position;   // �޷��� ��ġ
        animator.SetBool("Run", true);

        Vector3 targetDir = (momentTargetPosition - transform.position).normalized;
        targetDir = new Vector3(targetDir.x, 0, targetDir.z);                           // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.

        while (Vector3.Distance(momentTargetPosition, transform.position) >= 7f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDir.normalized), 2f * Time.deltaTime);
            transform.Translate(targetDir * 9 * Time.deltaTime, Space.World);           // Ÿ�������� �̵�
            yield return null;
        }

        animator.SetBool("Run", false);
        yield return null;
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

        
        if (name == "Stagger")  // �������� 
        {
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))   // �ִϸ��̼��� ��ȯ�ɶ����� ���
            {
                if (druidStatus.state == MONSTER_STATE.Dead)
                {
                    animator.SetBool("Dead", true);
                }
                yield return null;
            }

            if (druidStatus.state != MONSTER_STATE.Dead)
            {
                Debug.Log("state : " + druidStatus.state);
                startRoar = false;
                Init();
            }
            
        }

        yield return null;

    }
    IEnumerator CoolTime(float cool)
    {
        while (cool > 0)
        {
            cool -= Time.deltaTime;
            if (attackCoolTime)         // Ÿ�ٰ��� �Ÿ��� ��������� attackCoolTime�� true�� ������ �ڷ�ƾ ���߱�
                yield break;

            yield return null;
        }

        attackCoolTime = true;
    }
    #region �ִϸ��̼� �̺�Ʈ
    Projectile projectile;
    Vector3 momentTargetPosition;       // �� �ൿ�� �Ҷ� Ÿ���� ��ġ
    void CreateRock()           // �� ����
    {
        projectile = Instantiate(throwingObject, rightHand.position + rightHand.right, transform.rotation, rightHand).GetComponent<Rock>();
    }
    void ThrowRock()            // �� ������
    {
        momentTargetPosition = druidStatus.target.transform.position;       // ���� ������ Ÿ���� ��ġ�� ���� ������ ����
        projectile.Init(momentTargetPosition, 15, 35);                      // �Ű����� (Ÿ���� ��ġ, ���ݷ�, �ӵ�)
    }
    void Roar()
    {
        if (druidStatus.state.HasFlag(MONSTER_STATE.Attack))
        {
            attackParticle.SetActive(true);
        }
    }
    void AttackStart(MONSTER_ATTACK_TYPE attackType)
    {
        switch (attackType)
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
    public void StartStaggerState()
    {
        druidStatus.state = MONSTER_STATE.Stagger;
        animator.SetInteger("Rotation", 0);
        StopAllCoroutines();
        
        StartCoroutine(WaitForAnimation("Stagger", 1f));
    }

    public void Dead()
    {
        animator.SetBool("Dead", true);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Stagger"))
        {
            animator.SetTrigger("DeadTrigger");
        }

        druidStatus.state = MONSTER_STATE.Dead;
        animator.SetInteger("Rotation", 0);
        StopAllCoroutines();
    }
}
