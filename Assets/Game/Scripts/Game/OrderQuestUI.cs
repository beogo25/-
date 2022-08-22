using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderQuestUI : MonoBehaviour
{
    public Image           targetImage;
    public TextMeshProUGUI targetName;
    public TextMeshProUGUI targetNum;
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
        MainCanvas.instance.PlaySoundOneShot(MainCanvas.instance.questOpen.Path);
        Refresh();
    }
    public void Refresh()
    {
        if (player.orderQuest != null)
        {
            Quest quest = player.orderQuest ?? new Quest();
            targetImage.color = Color.white;
            //몬스터 리스트 내용 추가
            if (quest.collectionQuest)
            {
                targetImage.sprite = DataManager.instance.materialsDic[quest.target].sprite;
                targetName.text = quest.target;
                targetNum.text = WarehouseManager.instance.FindItem(DataManager.instance.materialsDic[quest.target]) + "/" + quest.targetNum.ToString();
                targetNum.color = Color.red;
                if (quest.targetNum <= WarehouseManager.instance.FindItem(DataManager.instance.materialsDic[quest.target]))
                {
                    clearButton.SetActive(true);
                    targetNum.color = Color.green;
                }
                else
                    clearButton.SetActive(false);
            }
            else
            {
                targetImage.sprite = DataManager.instance.monsterDataDic[quest.target].monaterSprite;
                targetName.text = DataManager.instance.monsterDataDic[quest.target].monsterName;
                targetNum.text = quest.targetNum - player.questCount+"/"+ quest.targetNum.ToString();
                if (player.questCount <= 0)
                {
                    clearButton.SetActive(true);
                    targetNum.color = Color.green;
                }
                else
                    clearButton.SetActive(false);
            }
            clearGold.text = "보상 : " + quest.clearGold.ToString() + "골드";
            questName.text = quest.questName;
            questContents.text = quest.questContents;
            
        }
        else
        {
            clearButton.SetActive(false);
            targetImage.color = Color.clear;
            targetNum.text = "";
            targetName.text = "";
            clearGold.text = "";
            questName.text = "";
            questContents.text = "퀘스트를 수주하지 않았습니다";
        }
    }

    public void QuestClear()
    {
        Quest quest = player.orderQuest ?? new Quest();
        if (quest.collectionQuest == true)
        {
            WarehouseManager.instance.MinusItem(DataManager.instance.materialsDic[quest.target], quest.targetNum);
            PlayerStatus.gold += quest.clearGold;
        }
        else
        {
            PlayerStatus.gold += quest.clearGold;
        }
        player.orderQuest = null;
        MainCanvas.instance.Exit();
        MainCanvas.instance.PlaySoundOneShot(MainCanvas.instance.questClear.Path);
    }
}
