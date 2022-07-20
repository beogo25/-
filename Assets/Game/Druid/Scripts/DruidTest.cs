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

    //�� ��Ƽ� ������ �Լ� �ؿ� �ִ� �ڷ�ƾ���� ��������� / �ִϸ��̼� �ȿ� ���� �ص�
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

    //���� ������� �ʿ� ���� �Լ�
    IEnumerator Throw(GameObject gameObject)
    {
        
        yield return new WaitForSeconds(1.5f);
        gameObject.transform.parent = null; //�θ���� ������ ������
        Vector3 targetLocation = (target.transform.position - gameObject.transform.position).normalized; //��ǥ�� ��ġ�� ���
        for(int i = 0; i < 200; i++)                                    //�ϴ��� �뷫 2�ʵ��� ���󰡰� �ص� / ���� ���� �ʿ�
        {
            gameObject.transform.Rotate(8f,3f,0);                       //�̰͵� ��~�� ���� ���� ��
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
        for (int i = 0; i < 200; i++)                                    //�ϴ��� �뷫 2�ʵ��� ���󰡰� �ص� / ���� ���� �ʿ�
        {
            gameObject.transform.GetChild(0).transform.Rotate(0, 0, 5);                       //�̰͵� ��~�� ���� ���� ��
            gameObject.transform.position +=  transform.right * 0.4f;
            yield return new WaitForSeconds(0.01f);

        }
        Destroy(gameObject);

    }
}
