using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TalkManager : Singleton<TalkManager>
{
    public  GameObject      talkUI;
    public  Image           standing;
    public  TextMeshProUGUI nameTMP;
    public  TextMeshProUGUI talkTMP;

    private string          npcName;
    private string[]        talkText;
    private int             num;
    private UIType[]        uiTypes;
    IEnumerator             talkIE;

    private WaitForSecondsRealtime talkDelay = new WaitForSecondsRealtime(0.1f);

    public GameObject[] UIButton = new GameObject[0];
    public Player player;


    private void Update()
    {
        if(Input.GetButtonUp("InteractionNpc") && talkUI.activeInHierarchy)
        {
            ClickButton();
        }
    }
    public void TalkStart(string name, string[] talk, Sprite sprite = null, UIType[] inputUITypes = null)
    {
        for(int i = 0; i < UIButton.Length-1; i++)
            UIButton[i].SetActive(false);

        UIButton[UIButton.Length-1].SetActive(true);

        player.TalkState = true;
        num      = 0;
        talkUI.SetActive(true);
        uiTypes  = inputUITypes; 
        npcName  = name;
        talkText = talk;
        if(sprite != null)
        {
            standing.gameObject.SetActive(true);
            standing.sprite = sprite;
        }
        else
            standing.gameObject.SetActive(false);
        talkIE = Talking(num);
        StartCoroutine(talkIE);
    }
    public void ClickButton()
    {
        if(talkTMP.text == talkText[num])
        {
            if(num+1 == talkText.Length)
            {
                if (uiTypes == null || uiTypes.Length == 0) 
                {
                    talkUI.SetActive(false);
                    //blur.SetActive(false);
                    UIButton[UIButton.Length - 1].SetActive(true);
                    player.TalkState = false;
                }
                else
                {
                    for(int i = 0; i < uiTypes.Length; i++)
                    {
                        UIButton[(int)uiTypes[i]].SetActive(true);
                    }
                    UIButton[7].SetActive(true);
                    GameManager.instance.eventSystem.SetSelectedGameObject(UIButton[(int)uiTypes[0]]);
                }
            }
            else
            {
                num++;
                talkIE = Talking(num);
                StartCoroutine(talkIE);
            }
        }
        else
        {
            nameTMP.text = npcName;
            talkTMP.text = talkText[num];
            StopCoroutine(talkIE);
        }
    }

    public IEnumerator Talking(int a)
    {
        nameTMP.text = npcName;
        talkTMP.text = null;
        for (int j = 0; j < talkText[a].Length; j++)
        {
            talkTMP.text += talkText[a][j];
            yield return talkDelay;
        }
    }
}
