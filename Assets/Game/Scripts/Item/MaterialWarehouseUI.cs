using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialWarehouseUI : WarehouseUI
{
    public override void ItemInformationChange(int num)
    {
        itemInformation.Item = WarehouseManager.instance.materialItemList[num];
        base.ItemInformationChange(num);
    }
    public override void Refresh()
    {
        int num = slots.Length;
        if (num > WarehouseManager.instance.materialItemList.Count)
            num = WarehouseManager.instance.materialItemList.Count;
        for (int i = 0; i < num; i++)
        {
            slots[i].image.color = Color.white;
            slots[i].image.sprite = WarehouseManager.instance.materialItemList[i].sprite;
            slots[i].stack.text = WarehouseManager.instance.materialItemList[i].stack.ToString();
            slots[i].button.interactable = true;
        }
        for (int i = num; i < slots.Length; i++)
        {
            slots[i].image.color = new Color(0,0,0,0);
            slots[i].stack.text = "";
            slots[i].button.interactable = false;
        }
    }
}
