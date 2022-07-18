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

            //처음 습득때 살짝 올라오면서 습득
            for (int i = 0; i < 10; i++)
            {
                pulledObject.transform.Translate(Vector3.up * 5 * GameManager.instance.ratio);
                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(3f);

            //없어질 때 살짝 올라오면서 없어짐
            for (int i = 0; i < 10; i++)
            {
                pulledObject.transform.Translate(Vector3.up * 5 * GameManager.instance.ratio);
                yield return new WaitForSeconds(0.01f);
            }
            pulledObject.SetActive(false);
            //위치 초기화
            pulledObject.transform.localPosition = Vector3.zero + new Vector3(-50, 50, 0);
            poolingObjects.Enqueue(pulledObject);

            
        }
        else
        {
            //위로 밀리는 효과를 줌, 사실은 전체가 위로 올라가는 효과임
            for (int i = 0; i < 10; i++)
            {
                transform.Translate(Vector3.up * 10 * GameManager.instance.ratio);
                yield return new WaitForSeconds(0.01f);
            }
            //pushTimes는 얼만큼 밀리는가에 대한 식이다 큐의 길이가 원래의 길이가 되면 다시 0으로 초기화를 해줌
            pushedTimes++;
            pulledObject.transform.Translate(Vector3.up * (pushedTimes) * -100 * GameManager.instance.ratio);

            pulledObject.SetActive (true);
            
            for (int i = 0; i < 10; i++)
            {
                pulledObject.transform.Translate(Vector3.up * 5 * GameManager.instance.ratio);
                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(3f);

            //없어질 때 살짝 올라오면서 없어짐
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
