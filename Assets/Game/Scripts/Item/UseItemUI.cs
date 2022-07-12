using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseItemUI : MonoBehaviour
{
    private int selectNum = 0;
    public Image leftItem;
    public Image middleItem;
    public Image rightItem;
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
            }
            else
            {
                middleItem.color = Color.white;
                rightItem.color = Color.white;
                leftItem.color = Color.white;
                if (selectNum == 0)
                {
                    middleItem.sprite = InventoryManager.instance.useItemList[0].sprite;
                    leftItem.sprite = InventoryManager.instance.useItemList[itemCount].sprite;
                    if (itemCount == 0)
                        rightItem.sprite = InventoryManager.instance.useItemList[0].sprite;
                    else
                        rightItem.sprite = InventoryManager.instance.useItemList[1].sprite;
                }
                else if (selectNum == itemCount)
                {
                    middleItem.sprite = InventoryManager.instance.useItemList[itemCount].sprite;
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
    public void ButtonAction(int num)
    {
        SelectNum += num;
    }
    private void OnEnable()
    {
        selectNum = 0;
    }
}
