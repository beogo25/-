using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderQuestUI : MonoBehaviour
{
    public Image monsterImage;
    public TextMeshProUGUI monsterName;
    public TextMeshProUGUI clearGold;
    public TextMeshProUGUI questName;
    public TextMeshProUGUI questContents;

    private Player player;
    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnEnable()
    {
        if(player.orderQuest != null)
        {
            Quest quest =player.orderQuest ?? new Quest();
            monsterImage.color = Color.white;
            //���� ����Ʈ ���� �߰�
            //monsterImage.sprite
            //monsterName.text = ;
            clearGold.text = "���� : " + quest.clearGold.ToString() + "���";
            questName.text = quest.questName;
            questContents.text = quest.questContents;
        }
        else
        {
            monsterImage.color = Color.clear;
            monsterName.text = "";
            clearGold.text = "";
            questName.text = "";
            questContents.text = "����Ʈ�� �������� �ʾҽ��ϴ�";
        }
    }
}
