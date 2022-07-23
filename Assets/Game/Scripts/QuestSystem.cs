using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestSystem : MonoBehaviour
{
    public  GameObject      contents;
    public  GameObject      contentsPrefab;
    private RectTransform   contentsRectTransform;

    public  Image           targetImage;
    public  TextMeshProUGUI targetName;
    public  TextMeshProUGUI targetNum;
    public  TextMeshProUGUI clearGold;
    public  TextMeshProUGUI questName;
    public  TextMeshProUGUI questContents;

    public  GameObject      orderButton;

    private int             target;

    public  Sprite          sword;
    private Player          player;
    private void Awake()
    {
        contentsRectTransform = contents.GetComponent<RectTransform>();
        player = FindObjectOfType<Player>();
        for (int i = 0; i < DataManager.instance.questList.Count; i++)
        {
            CombiContentsUI temp = Instantiate(contentsPrefab, contents.transform).GetComponent<CombiContentsUI>();
            temp.image.sprite = sword;
            temp.textMeshProUGUI.text = DataManager.instance.questList[i].questName;
            int tempint = i;
            temp.gameObject.GetComponent<Button>().onClick.AddListener(() => { QuestView(tempint); });
            contentsRectTransform.sizeDelta = new Vector2(contentsRectTransform.sizeDelta.x, contentsRectTransform.sizeDelta.y + 50);
        }
    }
    private void OnEnable()
    {
        GameManager.instance.eventSystem.SetSelectedGameObject(contents.transform.GetChild(0).transform.gameObject);
        orderButton.SetActive(false);
    }
    public void QuestView(int num)
    {
        target = num;
        Quest quest = DataManager.instance.questList[num];
        //monsterImage = ���͸���Ʈ[quest.targetMonster].image
        //monsterName = ���͸���Ʈ[quest.targetMonster].name
        if(quest.collectionQuest)
        {
            targetImage.sprite = DataManager.instance.materialsDic[quest.target].sprite;
            targetName.text = quest.target;
            targetNum.text = "0/" + quest.targetNum.ToString();
        }    
        else
        {
            targetImage.sprite = null;
            targetName.text = null;
            targetNum.text = null;
        }
        clearGold.text = "���� : "+quest.clearGold.ToString()+"���";
        questName.text = quest.questName;
        questContents.text = quest.questContents;
        orderButton.SetActive(true);
        GameManager.instance.eventSystem.SetSelectedGameObject(orderButton);
    }
    public void OrderButton()
    {
        //�����ߴٴ� UI �۵�
        player.orderQuest = DataManager.instance.questList[target];
    }
}
