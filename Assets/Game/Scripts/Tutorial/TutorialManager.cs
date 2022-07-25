using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public Image faceImage;
    public Sprite[] facialSprites = new Sprite[3];
    public TextMeshProUGUI textArea;
    private Transform[] spots = new Transform[4];
    private int number = 0;
    private bool isOpening = false;

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
            if (number < spots.Length - 1)
            {
                switch(num)
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
            else
                number = 0;

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
        Debug.Log("1단계");
        if(Input.GetKeyDown(KeyCode.H))
        {
            if(!isOpening)
            {
                Debug.Log("성공");
                StartCoroutine(OpenDoor());
            }
        }

    }

    void LockOnAttackTutorial()
    {
        Debug.Log("2단계");
        if(Player.isLockedOn)
        {
            if (!isOpening)
            {
                Debug.Log("성공");
                StartCoroutine(OpenDoor());
            }
        }
    }

    void CollectTutorial()
    {
        
    }

    void UseTutorial()
    {

    }

    IEnumerator OpenDoor()
    {
        isOpening = true;
        for(int i = 0; i < 50; i++)
        {
            transform.GetChild(number).GetChild(0).transform.Translate(Vector3.down * 0.5f, Space.World);
            yield return new WaitForSeconds(0.02f);
        }
        isOpening = false;
        number++;
    }
}