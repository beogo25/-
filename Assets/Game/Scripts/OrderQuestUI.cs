using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderQuestUI : MonoBehaviour
{
    public Image           monsterImage;
    public TextMeshProUGUI monsterName;
    public TextMeshProUGUI clearGold;
    public TextMeshProUGUI questName;
    public TextMeshProUGUI questContents;
    public GameObject      clearButton;

    private Player player;
    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnEnable()
    {
        Refresh();
    }
    public void Refresh()
    {
        if (player.orderQuest != null)
        {
            Quest quest = player.orderQuest ?? new Quest();
            monsterImage.color = Color.white;
            //몬스터 리스트 내용 추가
            if (quest.collectionQuest)
            {
                monsterImage.sprite = DataManager.instance.materialsDic[quest.target].sprite;
                monsterName.text = quest.target + "\n" + WarehouseManager.instance.FindItem(DataManager.instance.materialsDic[quest.target]) + "/" + quest.targetNum.ToString();
            }
            else
            {
                monsterImage.sprite = null;
                monsterName.text = null;
            }
            clearGold.text = "보상 : " + quest.clearGold.ToString() + "골드";
            questName.text = quest.questName;
            questContents.text = quest.questContents;
            if(quest.targetNum <= WarehouseManager.instance.FindItem(DataManager.instance.materialsDic[quest.target]))
                clearButton.SetActive(true);
            else
                clearButton.SetActive(false);
        }
        else
        {
            monsterImage.color = Color.clear;
            monsterName.text = "";
            clearGold.text = "";
            questName.text = "";
            questContents.text = "퀘스트를 수주하지 않았습니다";
        }
    }

    public void QuestClear()
    {
        Quest quest = player.orderQuest ?? new Quest();
        WarehouseManager.instance.MinusItem(DataManager.instance.materialsDic[quest.target], quest.targetNum);
        PlayerStatus.gold += quest.clearGold;
        player.orderQuest = null;
        Refresh();
    }
}
