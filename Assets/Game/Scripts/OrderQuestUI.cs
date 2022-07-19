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
            //몬스터 리스트 내용 추가
            //monsterImage.sprite
            //monsterName.text = ;
            clearGold.text = "보상 : " + quest.clearGold.ToString() + "골드";
            questName.text = quest.questName;
            questContents.text = quest.questContents;
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
}
