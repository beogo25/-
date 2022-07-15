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
    
    enum State
    {
        Idle = 1 << 0,
        Walk = 1 << 1,
        Rotation = 1 << 2,

        All = Walk | Rotation,
    }

    private MonsterBehaviorState behaviorState;
    private State state = 0;

    private Transform targetPlayer;
    private Animator animator;
    private Rigidbody monsterRigidbody;

    [SerializeField] private Transform moveDestination;
    [SerializeField] private float moveSpeed = 2;
    private float rotationSpeed;
    private Vector3 point = Vector3.zero;
    
    

    void Start()
    {
        animator = GetComponent<Animator>();
        monsterRigidbody = GetComponent<Rigidbody>();

        behaviorState = MonsterBehaviorState.SerchingTarget;
        StartCoroutine(behaviorState.ToString());
    }
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("�ȱ� ���� : " + state.HasFlag(State.Walk));
            animator.SetTrigger("Stretch");
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            //animator.SetTrigger("Test");
            
        }
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            animator.SetInteger("Rotation", -2);
            StartCoroutine(Rotation(-90));
        }
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            animator.SetInteger("Rotation", 2);
            StartCoroutine(Rotation(90));
        }
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            animator.SetInteger("Rotation", -1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            animator.SetInteger("Rotation", 1);
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            animator.SetInteger("Rotation", 3);
            StartCoroutine(Rotation(90));
        }
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            animator.SetInteger("Rotation", -3);
            StartCoroutine(Rotation(90));
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            animator.SetInteger("Rotation", 4);
            StartCoroutine(Rotation(90));
            //Debug.Log("�ִϸ��̼� ��� �ð� : " + animator.GetCurrentAnimatorStateInfo(0).length);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            animator.SetInteger("Rotation", -4);
            StartCoroutine(Rotation(-60));
            //Debug.Log("�ִϸ��̼� ��� �ð� : " + animator.GetCurrentAnimatorStateInfo(0).length);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            //animator.SetInteger("Rotation", 5);
            Debug.Log("�ִϸ��̼� ��� �ð� : " + animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }

    int SerchingTargetState = 0;
    Quaternion targetRotation;
    bool rotate = false;
    IEnumerator SerchingTarget()
    {
        while (true)
        {
            if (SerchingTargetState == 0)       // ��ǥ ������
            {
                if (!state.HasFlag(State.Rotation))
                {
                    state |= State.Rotation;
                    targetRotation = SetTargetDirection();
                }

                if (rotate)
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                if (Quaternion.Angle(transform.rotation, targetRotation) < 4)   // �� ���� ���̰� ����(4��) �����϶� ��ǥ�� ���� �ִ°����� �Ǵ�
                {
                    SerchingTargetState = 1;
                    state &= ~State.Rotation;
                    animator.SetInteger("Rotation", 0);
                    rotate = false;
                }
                
            }
            else if (SerchingTargetState == 1)  
            {
                if (Vector3.Distance(transform.position, moveDestination.position) < 4f)
                {
                    monsterRigidbody.velocity = Vector3.zero;
                    if (RandomPoint(moveDestination.position, 20f, out point))
                    {
                        moveDestination.position = point;
                        
                        state &= ~State.Walk;
                        animator.SetBool("Walk", state.HasFlag(State.Walk));

                        Debug.LogFormat("��ǥ�����Ϸ�1");
                        //yield return StartCoroutine(WaitForAnimation("Stretch", 1f));
                        //Debug.LogFormat("��ǥ�����Ϸ�2");
                        yield return new WaitForSecondsRealtime(Random.Range(0.3f,1.3f));
                        SerchingTargetState = 0;
                    }
                }
                else
                {
                    if (!state.HasFlag(State.Walk))
                    {
                        state |= State.Walk;
                        animator.SetBool("Walk", state.HasFlag(State.Walk));
                    }
                    
                    transform.Translate(transform.forward *moveSpeed* Time.deltaTime,Space.World);
                }
            }

            yield return null;
        }
    }
     
    IEnumerator Rotation(float targetAngle)
    {
        float time = animator.GetCurrentAnimatorStateInfo(0).length;


        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            time = 0.93f - 0.30f;
        }
        else
            time = time * 0.60f;
        
        for (int i = 0; i < 100; i++)
        {
            transform.Rotate(new Vector3(0, (targetAngle / 100),0), Space.Self);
            yield return new WaitForSecondsRealtime(time / 100f);
        }
    }
    Quaternion SetTargetDirection()
    {        
        Vector3 dir = moveDestination.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // ��ǥ������ �׺�޽�(��)�̴ϱ� Y���� 0���� �����ν� �ٴ��� ���� �ʵ��� ����.
        targetRotation = Quaternion.LookRotation(dir.normalized);   // ���� �ٶ� ����

        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation); // �� ����� ��ǥ ������ ��������

        Vector3 targetDir = moveDestination.position - transform.position; // Ÿ�� �������� ���ϴ� ���͸� ���ϱ�
        Vector3 crossVec = Vector3.Cross(targetDir, this.transform.forward); // ������� ����
        float dot = Vector3.Dot(crossVec, Vector3.up);
        if (dot > 0) // ����
        {
            if (angleDifference > 60)
            {
                animator.SetInteger("Rotation", -2);
                StartCoroutine(Rotation(-angleDifference));
            }
            else
            {
                
                animator.SetInteger("Rotation", -1);
                //StartCoroutine(Rotation(angleDifference));
                rotationSpeed = 1.2f;
                rotate = true;
            }
        }
        else if (dot < 0) // ������
        {
            if (angleDifference > 60)
            {
                animator.SetInteger("Rotation", 2);
                StartCoroutine(Rotation(angleDifference));
            }
            else
            {
                animator.SetInteger("Rotation", 1);
                //StartCoroutine(Rotation(angleDifference));
                rotationSpeed = 1.2f;
                rotate = true;
            }
        }
        else // ��� (0�϶�)
        {
            Debug.Log("���");
        }

        return targetRotation;
    }
    
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 40; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas) && Vector3.Distance(transform.position, randomPoint) > 6f)
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
    IEnumerator WaitForAnimation(string name, float ratio, int layer = -1)
    {
        //animator.Play(name, layer, 0);  // layer�� name�̸��� ���� �ִϸ��̼��� 0�ʺ��� �����ض�
        animator.SetTrigger(name);
        //Debug.Log("��Ʈ��ġ ���༺��!, " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < ratio && animator.GetCurrentAnimatorStateInfo(0).IsName(name))
            {
            //Debug.Log("��Ʈ��ġ ������ : " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            yield return new WaitForEndOfFrame();
        }
    }


    //ChangeState(WeaponState.AttackToTarget); 
    public void ChangeState(MonsterBehaviorState newState)           
    {
        StopCoroutine(behaviorState.ToString());              // ���� �������� �ڷ�ƾ ����

        behaviorState = newState;
        //Debug.Log("ChangeState! �ٲ� ���� : " + newState);
        StartCoroutine(behaviorState.ToString());             // ����� ���·� �ڷ�ƾ ����
    }

}