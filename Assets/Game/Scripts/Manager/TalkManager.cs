using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TalkManager : Singleton<TalkManager>
{
    public GameObject talkUI;
    public Image standing;
    public TextMeshProUGUI nameTMP;
    public TextMeshProUGUI talkTMP;

    private string npcName;
    private string[] talkText;
    private int num;
    private UIType[] uiTypes;
    IEnumerator talkIE;

    public GameObject useItemWarehouseButton;
    public GameObject equipmentItemWarehouseButton;
    public GameObject materialItemWarehouseButton;
    public GameObject exitButton;


    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Z))
        {
            ClickButton();
        }
    }
    public void TalkStart(string name, string[] talk, Sprite sprite = null, UIType[] inputUITypes = null)
    {
        uiTypes = inputUITypes; 
        num = 0;
        talkUI.SetActive(true);
        npcName = name;
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
                }
                else
                {
                    for(int i = 0; i < uiTypes.Length; i++)
                    {
                        switch (uiTypes[i])
                        {
                            case UIType.EquipmentUI:
                                equipmentItemWarehouseButton.SetActive(true);
                                break;
                            case UIType.MaterialUI:
                                materialItemWarehouseButton.SetActive(true);
                                break;
                            case UIType.UseItemUI:
                                useItemWarehouseButton.SetActive(true);
                                break;
                            default:
                                break;
                        }
                        exitButton.SetActive(true);
                    }
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
            yield return new WaitForSeconds(0.1f);
        }
    }
}
