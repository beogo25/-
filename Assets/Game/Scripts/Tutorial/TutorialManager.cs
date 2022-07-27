using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class KeySetting
{
    public string name;
    public string description;
    public bool isDone;
    public Sprite[] images = new Sprite[2];
}

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private Player player;
    public Image faceImage;
    public TextMeshProUGUI textArea;
    private Transform[] spots = new Transform[4];
    public KeySetting[] keySettings = new KeySetting[0];
    private int number = 0;
    
    
    private bool isOpening  = false;
    private bool isProvided = false;
    private bool isTextPlayed = false;
    
    [Header("이동 및 카메라")]
    [TextArea]
    public string[] linesA = new string[0];
    
    [Header("공격 및 락온")]
    [TextArea]
    public string[] linesB = new string[0];

    [Header("채집 및 제작")]
    [TextArea]
    public string[] linesC = new string[0];

    [Header("상태이상 및 아이템")]
    [TextArea]
    public string[] linesD = new string[0];

    public GameObject checkList;

    private void Start()
    {
        for (int i = 0; i < spots.Length; i++)
        {
            spots[i] = transform.GetChild(i).transform;
        }
    }

    private void Update()
    {
        TutorialCheck(number);



    }

    void TutorialCheck(int num)
    {
        Collider[] colliders = Physics.OverlapSphere(spots[num].position, 35, 1 << LayerMask.NameToLayer("Player"));
        if (colliders.Length != 0)
        {
            switch (num)
            {
                case 0:
                    MovementTutorial();
                    break;
                case 1:
                    LockOnAttackTutorial();
                    break;
                case 2:
                    CollectTutorial();
                    break;
                case 3:
                    UseTutorial();
                    break;
                default:
                    break;
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red * 0.2f;
        for (int i = 0; i < transform.childCount; i++)
            Gizmos.DrawSphere(transform.GetChild(i).position, 35f);
    }

    void MovementTutorial()
    {
        StartCoroutine(TextChange(linesA));
        

        for(int i = 0; i < 4; i++)
        {
            checkList.transform.GetChild(i).transform.gameObject.SetActive(true);
            checkList.transform.GetChild(i).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = keySettings[i].description;
            if (!GameManager.isJoyPadOn)
                checkList.transform.GetChild(i).transform.GetChild(3).GetComponent<Image>().sprite = keySettings[i].images[0];
            else
                checkList.transform.GetChild(i).transform.GetChild(3).GetComponent<Image>().sprite = keySettings[i].images[1];
        }

        if(Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            checkList.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(true);
            keySettings[0].isDone = true;
        }

        if(Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            checkList.transform.GetChild(1).transform.GetChild(1).transform.gameObject.SetActive(true);
            keySettings[1].isDone = true;
        }

        if((Input.GetButtonDown("Jump") && !GameManager.isJoyPadOn) || (Input.GetAxis("Jump") > 0 && GameManager.isJoyPadOn))
        {
            checkList.transform.GetChild(2).transform.GetChild(1).transform.gameObject.SetActive(true);
            keySettings[2].isDone = true;
        }

        if(Input.GetButtonDown("Evade") && player.isGround)
        {
            checkList.transform.GetChild(3).transform.GetChild(1).transform.gameObject.SetActive(true);
            keySettings[3].isDone = true;
        }


        if(keySettings[0].isDone && keySettings[1].isDone && keySettings[2].isDone && keySettings[3].isDone)
        {
            for(int i = 0; i < 4; i++)
            {
                checkList.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(false);
                checkList.transform.GetChild(i).gameObject.SetActive(false);
            }
            if (!isOpening)
            {
                Debug.Log("성공");
                StartCoroutine(OpenDoor());
            }
        }





    }

    void LockOnAttackTutorial()
    {
        StartCoroutine(TextChange(linesB));

        for (int i = 0; i < 4; i++)
        {
            checkList.transform.GetChild(i).transform.gameObject.SetActive(true);
            checkList.transform.GetChild(i).transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = keySettings[i + 4].description;
            if (!GameManager.isJoyPadOn)
                checkList.transform.GetChild(i).transform.GetChild(3).GetComponent<Image>().sprite = keySettings[i + 4].images[0];
            else
                checkList.transform.GetChild(i).transform.GetChild(3).GetComponent<Image>().sprite = keySettings[i + 4].images[1];
        }


        if (Input.GetButtonDown("Fire1"))
        {
            checkList.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(true);
            keySettings[4].isDone = true;
        }
        if (Input.GetButtonDown("Fire2"))
        {
            checkList.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(true);
            keySettings[5].isDone = true;
        }
        if (Input.GetButtonDown("R 3"))
        {
            checkList.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(true);
            keySettings[6].isDone = true;
        }

        if (Player.isLockedOn && Input.GetButtonDown("L Bumper"))
        {
            checkList.transform.GetChild(0).transform.GetChild(1).transform.gameObject.SetActive(true);
            keySettings[7].isDone = true;
        }



        if (keySettings[4].isDone && keySettings[5].isDone && keySettings[6].isDone && keySettings[7].isDone)
        {
            for (int i = 0; i < 4; i++)
                checkList.transform.GetChild(i).gameObject.SetActive(false);
            if (!isOpening)
            {
                Debug.Log("성공");
                StartCoroutine(OpenDoor());
            }
        }
    }

    void CollectTutorial()
    {
        Debug.Log("3단계");
        if(WarehouseManager.instance.useItemList.Count > 0)
        {
            if (!isOpening)
            {
                Debug.Log("성공");
                StartCoroutine(OpenDoor());
            }
        }
        
    }

    void UseTutorial()
    {
        Debug.Log("4단계");
        if(!isProvided)
        {
            player.status.PlayerHit(1, 0, Vector3.zero, AttackType.BURN);
            player.status.PlayerHit(1, 0, Vector3.zero, AttackType.POISON);
            InventoryManager.instance.AddItem(DataManager.instance.useItemDic["비약"]);
            InventoryManager.instance.AddItem(DataManager.instance.useItemDic["해독제"]);
            isProvided = true;
        }

        if (player.status.Ailment == 0 && player.status.Hp == player.status.maxHp)
        {
            if (!isOpening)
            {
                Debug.Log("성공");
                StartCoroutine(OpenDoor());
            }
        }
    }

    IEnumerator OpenDoor()
    {
        isOpening = true;
        textArea.text = "잘했어요! 다음 단계로 넘어가요";
        for(int i = 0; i < 50; i++)
        {
            transform.GetChild(number).GetChild(0).transform.Translate(Vector3.down * 0.5f, Space.World);
            yield return new WaitForSeconds(0.02f);
        }
        isOpening = false;
        if (number < spots.Length)
        {
            number++;
            isTextPlayed = false;
        }
        else
        {
            Debug.Log("튜토리얼 끝");
        }
    }

    IEnumerator TextChange(string[] strings)
    {
        if(!isTextPlayed)
        {
            isTextPlayed = true;
            for (int i = 0; i < strings.Length; i++)
            {
                textArea.text = strings[i];
                yield return new WaitForSeconds(4f);
            }
        }
    }
}