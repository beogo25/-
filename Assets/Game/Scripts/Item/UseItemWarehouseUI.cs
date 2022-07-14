using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItemWarehouseUI : WarehouseUI
{
    public override void ItemInformationChange(int num)
    {
        itemInformation.Item = WarehouseManager.instance.useItemList[num];
        base.ItemInformationChange(num);
    }
    public override void Refresh()
    {
        int num = slots.Length;
        if (num > WarehouseManager.instance.useItemList.Count)
            num = WarehouseManager.instance.useItemList.Count;
        for (int i = 0; i < num; i++)
        {
            slots[i].image.sprite = WarehouseManager.instance.useItemList[i].sprite;
            slots[i].stack.text = WarehouseManager.instance.useItemList[i].stack.ToString();
            slots[i].gameObject.SetActive(true);
        }
        for (int i = num; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
    }
}
