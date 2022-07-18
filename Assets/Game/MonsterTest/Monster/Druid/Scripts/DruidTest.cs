using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DruidTest: MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Transform rightHand;
    [SerializeField] private GameObject[] objects = new GameObject[0];
    [SerializeField] private GameObject target;



    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            animator.SetTrigger("A");
        if (Input.GetKeyDown(KeyCode.S))
            animator.SetTrigger("B");        
        if (Input.GetKeyDown(KeyCode.D))
            StartCoroutine(GetObject(0));
        if (Input.GetKeyDown(KeyCode.F))
            StartCoroutine(GetObject(1)); 
    }

    //돌 잡아서 던지는 함수 밑에 있는 코루틴까지 실행시켜줌 / 애니메이션 안에 연결 해둠
    IEnumerator GetObject(int num)
    {
        animator.SetTrigger("C");
        yield return new WaitForSeconds(0.9f);
        GameObject myObject = Instantiate(objects[num], rightHand.position,transform.rotation,rightHand);
        
        if(num == 0)
            StartCoroutine(Throw(myObject));
        else if(num == 1)
            StartCoroutine(Swing(myObject));
    }

    //따로 사용해줄 필요 없는 함수
    IEnumerator Throw(GameObject gameObject)
    {
        
        yield return new WaitForSeconds(1.5f);
        gameObject.transform.parent = null; //부모와의 결합을 끊어줌
        Vector3 targetLocation = (target.transform.position - gameObject.transform.position).normalized; //목표의 위치를 잡고
        for(int i = 0; i < 200; i++)                                    //일단은 대략 2초동안 날라가게 해둠 / 추후 수정 필요
        {
            gameObject.transform.Rotate(8f,3f,0);                       //이것도 대~충 돌이 돌게 함
            gameObject.transform.position += targetLocation * 0.25f;
            yield return new WaitForSeconds(0.01f);
            
        }
        Destroy(gameObject);
    }
    
    IEnumerator Swing(GameObject gameObject)
    {
        animator.SetTrigger("D");
        yield return new WaitForSeconds(3.80f);
        gameObject.transform.parent = null;
        for (int i = 0; i < 200; i++)                                    //일단은 대략 2초동안 날라가게 해둠 / 추후 수정 필요
        {
            gameObject.transform.GetChild(0).transform.Rotate(0, 0, 5);                       //이것도 대~충 돌이 돌게 함
            gameObject.transform.position +=  transform.right * 0.4f;
            yield return new WaitForSeconds(0.01f);

        }
        Destroy(gameObject);

    }
}
