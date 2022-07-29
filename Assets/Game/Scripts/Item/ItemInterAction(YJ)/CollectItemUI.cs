using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CollectItemUI : MonoBehaviour
{
    private Image             getSprite;
    private TextMeshProUGUI   getName;
    private Vector3           firstPosition;
    private int               pushedTimes = 0;

    [SerializeField] 
    private GameObject        childObject;
    
    private Queue<GameObject> poolingObjects = new Queue<GameObject>();
    
    private void Awake()
    {
        for(int i = 0; i < 10; i++)
        {
            GameObject newObject = Instantiate(childObject, transform);
            newObject.SetActive(false);
            poolingObjects.Enqueue(newObject);
        }

        firstPosition = transform.localPosition;
    }

    private void Start()
    {
        WarehouseManager.instance.itemDelegate += SetItemDisplay;
        
    }
    private void SetItemDisplay(Item item)
    {
        GameObject pulledObject = poolingObjects.Dequeue();
        getSprite               = pulledObject.transform.GetChild(0).transform.gameObject.GetComponent<Image>();
        getName                 = pulledObject.transform.GetChild(1).transform.gameObject.GetComponent<TextMeshProUGUI>();
        getName.text            = item.itemName;
        getSprite.sprite        = item.sprite;
        StartCoroutine(Display(pulledObject));
    }

    private IEnumerator Display(GameObject pulledObject)
    {
        
        if(poolingObjects.Count == 9)
        {
            pushedTimes             = 0;
            transform.localPosition = firstPosition;
            pulledObject.SetActive(true);

            //ó�� ���涧 ��¦ �ö���鼭 ����
            for (int i = 0; i < 10; i++)
            {
                pulledObject.transform.Translate(Vector3.up * 5 * GameManager.instance.ratio);
                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(3f);

            //������ �� ��¦ �ö���鼭 ������
            for (int i = 0; i < 10; i++)
            {
                pulledObject.transform.Translate(Vector3.up * 5 * GameManager.instance.ratio);
                yield return new WaitForSeconds(0.01f);
            }
            pulledObject.SetActive(false);
            //��ġ �ʱ�ȭ
            pulledObject.transform.localPosition = Vector3.zero + new Vector3(-50, 50, 0);
            poolingObjects.Enqueue(pulledObject);

            
        }
        else
        {
            //���� �и��� ȿ���� ��, ����� ��ü�� ���� �ö󰡴� ȿ����
            for (int i = 0; i < 10; i++)
            {
                transform.Translate(Vector3.up * 10 * GameManager.instance.ratio);
                yield return new WaitForSeconds(0.01f);
            }
            //pushTimes�� ��ŭ �и��°��� ���� ���̴� ť�� ���̰� ������ ���̰� �Ǹ� �ٽ� 0���� �ʱ�ȭ�� ����
            pushedTimes++;
            pulledObject.transform.Translate(Vector3.up * (pushedTimes) * -100 * GameManager.instance.ratio);

            pulledObject.SetActive (true);
            
            for (int i = 0; i < 10; i++)
            {
                pulledObject.transform.Translate(Vector3.up * 5 * GameManager.instance.ratio);
                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(3f);

            //������ �� ��¦ �ö���鼭 ������
            for (int i = 0; i < 10; i++)
            {
                pulledObject.transform.Translate(Vector3.up * 5 * GameManager.instance.ratio);
                yield return new WaitForSeconds(0.01f);
            }
            pulledObject.SetActive(false);
            pulledObject.transform.localPosition = Vector3.zero + new Vector3 (-50, 50, 0);
            poolingObjects.Enqueue(pulledObject);
        }
    }
}
