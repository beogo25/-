using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentInventoryUI : WarehouseUI
{
    public override void Refresh()
    {
        int num = slots.Length;
        for (int i = 0; i < slots.Length; i++)
        {
            if (InventoryManager.instance.equipmentList[i] == null)
            {
                slots[i].image.color = new Color(0, 0, 0, 0);
                slots[i].stack.text = "";
                slots[i].button.interactable = false;
            }
            else
            {
                slots[i].image.color = Color.white;
                slots[i].image.sprite = WarehouseManager.instance.equipmentList[i].sprite;
                slots[i].stack.text = WarehouseManager.instance.equipmentList[i].itemName;
                slots[i].button.interactable = true;
            }
        }
    }

    public override void ItemInformationChange(int num)
    {
        itemInformation.Item = InventoryManager.instance.equipmentList[num];
        itemInformation.WareHouseBool = false;
        itemInformation.targetNum = num;
    }
}
