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
                slots[i].gameObject.SetActive(false);
            }
            else
            {
                slots[i].image.sprite = InventoryManager.instance.equipmentList[i].sprite;
                slots[i].gameObject.SetActive(true);
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
