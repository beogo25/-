using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestSystem : MonoBehaviour
{
    public GameObject contents;
    public GameObject contentsPrifab;
    private RectTransform contentsRectTransform;

    public Image monsterImage;
    public TextMeshProUGUI monsterName;
    public TextMeshProUGUI clearGold;
    public TextMeshProUGUI questName;
    public TextMeshProUGUI questContents;

    public GameObject orderButton;

    private int target;
    private Player player;
    public GameObject blur;

    public Sprite sword;
    private void Awake()
    {
        contentsRectTransform = contents.GetComponent<RectTransform>();
        player = FindObjectOfType<Player>();
    }
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
            ExitButton();
    }
    void Start()
    {
        for (int i = 0; i < DataManager.instance.questList.Count; i++)
        {
            CombiContentsUI temp = Instantiate(contentsPrifab, contents.transform).GetComponent<CombiContentsUI>();
            temp.image.sprite = sword;
            temp.textMeshProUGUI.text = DataManager.instance.questList[i].questName;
            int tempint = i;
            temp.gameObject.GetComponent<Button>().onClick.AddListener(() => { QuestView(tempint); });
            contentsRectTransform.sizeDelta = new Vector2(contentsRectTransform.sizeDelta.x, contentsRectTransform.sizeDelta.y + 50);
        }
        orderButton.SetActive(false);
    }
    public void QuestView(int num)
    {
        target = num;
        Quest quest = DataManager.instance.questList[num];
        //monsterImage = ���͸���Ʈ[quest.targetMonster].image
        //monsterName = ���͸���Ʈ[quest.targetMonster].name
        clearGold.text = "���� : "+quest.clearGold.ToString()+"���";
        questName.text = quest.questName;
        questContents.text = quest.questContents;
        orderButton.SetActive(true);
    }
    public void OrderButton()
    {
        //����Ʈ ������ ��򰡷� ������ ����Ʈ ����
        ExitButton();
    }
    public void ExitButton()
    {
        gameObject.SetActive(false);
        blur.SetActive(false);
        player.talkState = false;
    }
}
