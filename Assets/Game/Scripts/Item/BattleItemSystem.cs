using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleItemSystem : MonoBehaviour
{
    private int             selectNum = 0;
    public  Image           leftItem;
    public  Image           middleItem;
    public  Image           rightItem;
    public  TextMeshProUGUI stackText;
    public  PlayerStatus    playerStatus;
    public  Image           cooltimeImage;
    private bool            isScrollMovable = true;

    private WaitForFixedUpdate oneFrame = new WaitForFixedUpdate();
    public int SelectNum
    {
        get { return selectNum; }
        set
        {
            int itemCount = InventoryManager.instance.ItemCount-1;
            if (value> itemCount)
                selectNum = 0;
            else if(value < 0)
                selectNum = itemCount;
            else
                selectNum = value;
            if(itemCount < 0)
            {
                middleItem.color = new Color(0, 0, 0, 0);
                rightItem.color = new Color(0, 0, 0, 0);
                leftItem.color = new Color(0, 0, 0, 0);
                stackText.text = "";
            }
            else
            {
                middleItem.color = Color.white;
                rightItem.color = Color.white;
                leftItem.color = Color.white;
                stackText.text = InventoryManager.instance.useItemList[selectNum].stack.ToString();
                if (selectNum == 0)
                {
                    middleItem.sprite = InventoryManager.instance.useItemList[selectNum].sprite;
                    leftItem.sprite = InventoryManager.instance.useItemList[itemCount].sprite;
                    if (itemCount == 0)
                        rightItem.sprite = InventoryManager.instance.useItemList[0].sprite;
                    else
                        rightItem.sprite = InventoryManager.instance.useItemList[1].sprite;
                }
                else if (selectNum == itemCount)
                {
                    middleItem.sprite = InventoryManager.instance.useItemList[selectNum].sprite;
                    rightItem.sprite = InventoryManager.instance.useItemList[0].sprite;
                    if (itemCount == 0)
                        leftItem.sprite = InventoryManager.instance.useItemList[0].sprite;
                    else
                        leftItem.sprite = InventoryManager.instance.useItemList[itemCount-1].sprite;
                }
                else
                {
                    middleItem.sprite = InventoryManager.instance.useItemList[selectNum].sprite;
                    rightItem.sprite = InventoryManager.instance.useItemList[selectNum + 1].sprite;
                    leftItem.sprite = InventoryManager.instance.useItemList[selectNum - 1].sprite;
                }
            }
        }
    }
    private void OnEnable()
    {
        selectNum = 0;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || (Input.GetAxis("D-Pad X") == 1 && isScrollMovable))
        {
            SelectNum -= 1;
            isScrollMovable = false;
        }
        if (Input.GetButtonDown("Button X") || Input.GetAxis("D-Pad Y") == 1)
        {
            UseItemTrigger();
            StartCoroutine(CooltimeCheck());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || (Input.GetAxis("D-Pad X") == -1 && isScrollMovable))
        {
            SelectNum += 1;
            isScrollMovable = false;
        }
        if(Input.GetAxis("D-Pad X") == 0)
        {
            isScrollMovable = true;
        }
    }

    //사용 아이템들 효과 적용
    public void UseItemTrigger()
    {
        if (cooltimeImage.fillAmount <= 0 && InventoryManager.instance.useItemList.Length>0)
        {
            if(InventoryManager.instance.useItemList[selectNum] != null )
            {
                playerStatus.UseItemEffect(InventoryManager.instance.useItemList[selectNum].useItemType, InventoryManager.instance.useItemList[selectNum].effectValue);
                InventoryManager.instance.MinusItem(SelectNum, 1);
                StartCoroutine(CooltimeCheck());
            }
        }
    }
    
    public IEnumerator CooltimeCheck()
    {
        cooltimeImage.fillAmount = 1;
        while(cooltimeImage.fillAmount > 0)
        {
            cooltimeImage.fillAmount -= 0.01f;
            yield return oneFrame; 
        }
    }
}
