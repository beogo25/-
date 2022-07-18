using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : WarehouseUI
{
    public override void Refresh()
    {
        int num = slots.Length;
        for (int i = 0; i < slots.Length; i++)
        {
            if (InventoryManager.instance.useItemList[i] == null)
            {
                slots[i].image.color = new Color(0, 0, 0, 0);
                slots[i].stack.text = "";
                slots[i].button.interactable = false;
            }
            else
            {
                slots[i].image.color = Color.white;
                slots[i].image.sprite = InventoryManager.instance.useItemList[i].sprite;
                slots[i].stack.text = InventoryManager.instance.useItemList[i].stack.ToString();
                slots[i].button.interactable = true;
            }
        }
    }

    public override void ItemInformationChange(int num)
    {
        itemInformation.Item = InventoryManager.instance.useItemList[num];
        itemInformation.WareHouseBool = false;
        itemInformation.targetNum = num;
    }
}
