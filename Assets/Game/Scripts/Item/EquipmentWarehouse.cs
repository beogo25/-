using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentWarehouse : Warehouse
{
    private void OnEnable()
    {
        int num = slots.Length;
        if (num > WarehouseManager.instance.equipmentList.Count)
            num = WarehouseManager.instance.equipmentList.Count;
        for (int i = 0; i < num; i++)
        {
            slots[i].image.sprite = WarehouseManager.instance.equipmentList[i].sprite;
            slots[i].gameObject.SetActive(true);
        }
        for (int i = num; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
    }

    public override void ItemInformationChange(int num)
    {
        itemInformation.image.sprite = WarehouseManager.instance.equipmentList[num].sprite;
        itemInformation.itemName.text = WarehouseManager.instance.equipmentList[num].itemName;
        itemInformation.contents.text = WarehouseManager.instance.equipmentList[num].contents;
    }
}
