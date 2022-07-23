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
        Refresh();
    }
    public void Refresh()
    {
        if (player.orderQuest != null)
        {
            Quest quest = player.orderQuest ?? new Quest();
            targetImage.color = Color.white;
            //���� ����Ʈ ���� �߰�
            if (quest.collectionQuest)
            {
                targetImage.sprite = DataManager.instance.materialsDic[quest.target].sprite;
                targetName.text = quest.target;
                targetNum.text = WarehouseManager.instance.FindItem(DataManager.instance.materialsDic[quest.target]) + "/" + quest.targetNum.ToString();
                targetNum.color = Color.red;
            }
            else
            {
                targetImage.sprite = null;
                targetName.text = null;
                targetNum.text = null;
            }
            clearGold.text = "���� : " + quest.clearGold.ToString() + "���";
            questName.text = quest.questName;
            questContents.text = quest.questContents;
            if(quest.targetNum <= WarehouseManager.instance.FindItem(DataManager.instance.materialsDic[quest.target]))
            {
                clearButton.SetActive(true);
                targetNum.color = Color.green;
            }
            else
                clearButton.SetActive(false);
        }
        else
        {
            targetImage.color = Color.clear;
            targetName.text = "";
            clearGold.text = "";
            questName.text = "";
            questContents.text = "����Ʈ�� �������� �ʾҽ��ϴ�";
        }
    }

    public void QuestClear()
    {
        Quest quest = player.orderQuest ?? new Quest();
        WarehouseManager.instance.MinusItem(DataManager.instance.materialsDic[quest.target], quest.targetNum);
        PlayerStatus.gold += quest.clearGold;
        player.orderQuest = null;
        gameObject.SetActive(false);    
    }
}
