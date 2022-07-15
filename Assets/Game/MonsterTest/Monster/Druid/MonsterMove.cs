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
            Debug.Log("걷기 상태 : " + state.HasFlag(State.Walk));
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
            //Debug.Log("애니메이션 재생 시간 : " + animator.GetCurrentAnimatorStateInfo(0).length);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            animator.SetInteger("Rotation", -4);
            StartCoroutine(Rotation(-60));
            //Debug.Log("애니메이션 재생 시간 : " + animator.GetCurrentAnimatorStateInfo(0).length);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            //animator.SetInteger("Rotation", 5);
            Debug.Log("애니메이션 재생 시간 : " + animator.GetCurrentAnimatorStateInfo(0).length);
        }
    }

    int SerchingTargetState = 0;
    Quaternion targetRotation;
    bool rotate = false;
    IEnumerator SerchingTarget()
    {
        while (true)
        {
            if (SerchingTargetState == 0)       // 목표 방향중
            {
                if (!state.HasFlag(State.Rotation))
                {
                    state |= State.Rotation;
                    targetRotation = SetTargetDirection();
                }

                if (rotate)
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                if (Quaternion.Angle(transform.rotation, targetRotation) < 4)   // 두 각의 차이가 일정(4도) 이하일때 목표를 보고 있는것으로 판단
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

                        Debug.LogFormat("목표도착완료1");
                        //yield return StartCoroutine(WaitForAnimation("Stretch", 1f));
                        //Debug.LogFormat("목표도착완료2");
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
        dir = new Vector3(dir.x, 0, dir.z);                         // 목표지점은 네브메쉬(땅)이니깐 Y축을 0으로 함으로써 바닥을 보지 않도록 해줌.
        targetRotation = Quaternion.LookRotation(dir.normalized);   // 내가 바라볼 방향

        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation); // 내 방향과 목표 방향의 각도차이

        Vector3 targetDir = moveDestination.position - transform.position; // 타겟 방향으로 향하는 벡터를 구하기
        Vector3 crossVec = Vector3.Cross(targetDir, this.transform.forward); // 포워드와 외적
        float dot = Vector3.Dot(crossVec, Vector3.up);
        if (dot > 0) // 왼쪽
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
        else if (dot < 0) // 오른쪽
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
        else // 가운데 (0일때)
        {
            Debug.Log("가운데");
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
        //animator.Play(name, layer, 0);  // layer에 name이름을 가진 애니메이션을 0초부터 시작해라
        animator.SetTrigger(name);
        //Debug.Log("스트렛치 실행성공!, " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < ratio && animator.GetCurrentAnimatorStateInfo(0).IsName(name))
            {
            //Debug.Log("스트렛치 실행중 : " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            yield return new WaitForEndOfFrame();
        }
    }


    //ChangeState(WeaponState.AttackToTarget); 
    public void ChangeState(MonsterBehaviorState newState)           
    {
        StopCoroutine(behaviorState.ToString());              // 기존 실행중인 코루틴 정지

        behaviorState = newState;
        //Debug.Log("ChangeState! 바뀔 상태 : " + newState);
        StartCoroutine(behaviorState.ToString());             // 변경된 상태로 코루틴 시작
    }

}