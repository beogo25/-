using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MonsterMoveTest : MonoBehaviour
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

    [SerializeField] public Transform moveDestination;
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


    int SerchingTargetState = 0;
    Quaternion targetRotation;
    IEnumerator SerchingTarget()
    {
        while (true)
        {
            if (SerchingTargetState == 0)       // 목표 방향중
            {
                bool rotate = true;


                //state |= State.Rotation;
                targetRotation = SetTargetDirection(out rotate);


                //if (rotate)
                //    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

                if (Quaternion.Angle(transform.rotation, targetRotation) < 7)   // 두 각의 차이가 일정(7도) 이하일때 목표를 보고 있는것으로 판단
                {
                    // SerchingTargetState = 1;
                    state &= ~State.Rotation;
                    animator.SetInteger("Rotation", 0);

                    Debug.Log("목표 방향 완료");
                    //yield return new WaitForSecondsRealtime(0.3f);
                }

            }

            yield return null;
        }
    }
    Quaternion SetTargetDirection(out bool rotate)
    {
        //Debug.Log("목표 방향 찾기 시작");

        Vector3 dir = moveDestination.position - transform.position;
        dir = new Vector3(dir.x, 0, dir.z);                         // 목표지점은 네브메쉬(땅)이니깐 Y축을 0으로 함으로써 바닥을 보지 않도록 해줌.
        targetRotation = Quaternion.LookRotation(dir.normalized);



        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);

        Vector3 targetDir = moveDestination.position - transform.position; // 타겟 방향으로 향하는 벡터를 구하기
        Vector3 crossVec = Vector3.Cross(targetDir, this.transform.forward); // 포워드와 외적
        float dot = Vector3.Dot(crossVec, Vector3.up);
        if (dot > 0) // 왼쪽
        {
            Debug.Log("왼쪽");
        }
        else if (dot < 0) // 오른쪽
        {
            Debug.Log("오른쪽");
        }
        else // 0 이라면 평행하기 때문에 가운데라고 적어두었다.
        {
            Debug.Log("가운데");
        }

        if (angleDifference > 120)
        {
            //Debug.Log("각도 : " + angleDifference + ", 전환속도 : 2 설정");
            rotationSpeed = 1f;
            //Debug.LogFormat("내 방향 : {0}, 목표 방향 : {1}",  transform.rotation.y, targetRotation.y);


            if (targetRotation.y > transform.rotation.y)
            {
                //animator.SetInteger("Rotation", 2);
                Debug.Log("각도 : " + angleDifference + ", 오른쪽 방향으로! " + "목표 방향 : " + Quaternion.Euler(0, targetRotation.y, 0) + ", 내 방향 : " + Quaternion.Euler(0, transform.rotation.y, 0));
                
            }
            else
            {
                Debug.Log("각도 : " + angleDifference + ", 왼쪽 방향으로! " + "목표 방향 : " + Quaternion.Euler(0, targetRotation.y, 0) + ", 내 방향 : " + Quaternion.Euler(0, transform.rotation.y, 0));
                //animator.SetInteger("Rotation", -2);
            }
            rotate = true;
        }
        else if (angleDifference > 60)
        {
            //Debug.Log("각도 : " + angleDifference + ", 전환속도 : 1.3 설정");
            rotationSpeed = 1f;

            if (targetRotation.y > transform.rotation.y)
            {
                Debug.Log("각도 : " + angleDifference + ", 오른쪽 방향으로!, " + "목표 방향 : " + Quaternion.Euler(0, targetRotation.y,0)  + ", 내 방향 : " + Quaternion.Euler(0, transform.rotation.y, 0));
                //animator.SetInteger("Rotation", 2);
            }
            else
            {
                Debug.Log("각도 : " + angleDifference + ", 왼쪽 방향으로! " + "목표 방향 : " + Quaternion.Euler(0, targetRotation.y, 0) + ", 내 방향 : " + Quaternion.Euler(0, transform.rotation.y, 0));
                //animator.SetInteger("Rotation", -2);
            }
            rotate = true;
        }
        else
        {
            //Debug.Log("각도 : " + angleDifference + ", 전환속도 : 0.8 설정");
            rotationSpeed = 0.8f;

            if (targetRotation.y > transform.rotation.y)
            {
                Debug.Log("각도 : " + angleDifference + ", 오른쪽 방향으로! " + "목표 방향 : " + Quaternion.Euler(0, targetRotation.y, 0) + ", 내 방향 : " + Quaternion.Euler(0, transform.rotation.y, 0));
                //animator.SetInteger("Rotation", 1);
                
            }
            else
            {
                Debug.Log("각도 : " + angleDifference + ", 왼쪽 방향으로! " + "목표 방향 : " + Quaternion.Euler(0, targetRotation.y, 0) + ", 내 방향 : " + Quaternion.Euler(0, transform.rotation.y, 0));
                //animator.SetInteger("Rotation", -1);
            }

            rotate = true;
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
        Debug.Log("스트렛치 실행성공!, " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < ratio)
        {
            Debug.Log("스트렛치 실행중 : " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
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