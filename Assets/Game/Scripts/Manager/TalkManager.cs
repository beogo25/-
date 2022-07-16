using System.Collections;
using TMPro;
using UnityEngine;
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
    public GameObject useItemConbinationButton;
    public GameObject equipmentItemConbinationButton;
    public GameObject shopButton;
    public GameObject exitButton;

    public Player player;

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Z))
        {
            ClickButton();
        }
    }
    public void TalkStart(string name, string[] talk, Sprite sprite = null, UIType[] inputUITypes = null)
    {
        player.talkState = true;
        num = 0;
        talkUI.SetActive(true);
        uiTypes = inputUITypes; 
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
                    player.talkState = false;
                }
                else
                {
                    for(int i = 0; i < uiTypes.Length; i++)
                    {
                        switch (uiTypes[i])
                        {
                            case UIType.EQIUPMENT_WAREHOUSE_UI:
                                equipmentItemWarehouseButton.SetActive(true);
                                break;
                            case UIType.MATERIAL_WAREHOUSE_UI:
                                materialItemWarehouseButton.SetActive(true);
                                break;
                            case UIType.USEITEM_WAREHOUSE_UI:
                                useItemWarehouseButton.SetActive(true);
                                break;
                            case UIType.USEITEM_COMBINATION_UI:
                                useItemConbinationButton.SetActive(true);
                                break;
                            case UIType.EQIUPMENT_COMBINATION_UI:
                                equipmentItemConbinationButton.SetActive(true);
                                break;
                            case UIType.SHOP_UI:
                                shopButton.SetActive(true);
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

    public void ExiteButton()
    {
        exitButton.SetActive(false);
        shopButton.SetActive(false);
        equipmentItemConbinationButton.SetActive(false);
        useItemConbinationButton.SetActive(false);
        useItemWarehouseButton.SetActive(false);
        materialItemWarehouseButton.SetActive(false);
        talkUI.SetActive(false);
        player.talkState = false;
    }
}
