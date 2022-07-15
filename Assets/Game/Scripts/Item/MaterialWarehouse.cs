using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialWarehouse : Warehouse
{
    private void OnEnable()
    {
        int num = slots.Length;
        if (num > WarehouseManager.instance.materialItemList.Count)
            num = WarehouseManager.instance.materialItemList.Count;
        for (int i = 0; i < num; i++)
        {
            slots[i].image.sprite = WarehouseManager.instance.materialItemList[i].sprite;
            slots[i].stack.text = WarehouseManager.instance.materialItemList[i].stack.ToString();
            slots[i].gameObject.SetActive(true);
        }
        for (int i = num; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
    }

    public override void ItemInformationChange(int num)
    {
        Debug.Log(num);
        itemInformation.image.sprite  = WarehouseManager.instance.materialItemList[num].sprite;
        itemInformation.itemName.text = WarehouseManager.instance.materialItemList[num].itemName;
        itemInformation.stack.text    = WarehouseManager.instance.materialItemList[num].stack.ToString();
        itemInformation.contents.text = WarehouseManager.instance.materialItemList[num].contents;
    }
}
