using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Warehouse
{
    public override void Refresh()
    {
        int num = slots.Length;
        for (int i = 0; i < slots.Length; i++)
        {
            if (InventoryManager.instance.useItemList[i] == null)
            {
                slots[i].gameObject.SetActive(false);
            }
            else
            {
                slots[i].image.sprite = InventoryManager.instance.useItemList[i].sprite;
                slots[i].stack.text = InventoryManager.instance.useItemList[i].stack.ToString();
                slots[i].gameObject.SetActive(true);
            }
        }
    }

    public override void ItemInformationChange(int num)
    {
        Debug.Log(num);
        itemInformation.Item = InventoryManager.instance.useItemList[num];
        itemInformation.WareHouseBool = false;
        itemInformation.targetNum = num;
    }
}
