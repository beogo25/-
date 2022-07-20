using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentWarehouseUI : WarehouseUI
{
    public override void ItemInformationChange(int num)
    {
        itemInformation.Item = WarehouseManager.instance.equipmentList[num];
        base.ItemInformationChange(num);
    }
    public override void Refresh()
    {
        int num = slots.Length;
        if (num > WarehouseManager.instance.equipmentList.Count)
            num = WarehouseManager.instance.equipmentList.Count;
        for (int i = 0; i < num; i++)
        {
            slots[i].image.color = Color.white;
            slots[i].image.sprite = WarehouseManager.instance.equipmentList[i].sprite;
            slots[i].stack.text = "";
            slots[i].button.interactable = true;
        }
        for (int i = num; i < slots.Length; i++)
        {
            slots[i].image.color = Color.clear;
            slots[i].stack.text = "";
            slots[i].button.interactable = false;
        }
    }
}
