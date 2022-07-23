using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseItemWarehouseUI : WarehouseUI
{
    public Button        itemInventorySlot;
    public override void ItemInformationChange(int num)
    {
        itemInformation.Item = WarehouseManager.instance.useItemList[num];
        base.ItemInformationChange(num);
    }
    public override void OnEnable()
    {
        base.OnEnable();
        if (itemInventorySlot.interactable == true)
            GameManager.instance.eventSystem.SetSelectedGameObject(slots[0].transform.GetChild(2).transform.gameObject);
        else
            GameManager.instance.eventSystem.SetSelectedGameObject(itemInventorySlot.transform.gameObject);
    }

    public override void Refresh()
    {
        int num = slots.Length;
        if (num > WarehouseManager.instance.useItemList.Count)
            num = WarehouseManager.instance.useItemList.Count;
        for (int i = 0; i < num; i++)
        {
            slots[i].image.color         = Color.white;
            slots[i].image.sprite        = WarehouseManager.instance.useItemList[i].sprite;
            slots[i].stack.text          = WarehouseManager.instance.useItemList[i].stack.ToString();
            slots[i].button.interactable = true;
        }
        for (int i = num; i < slots.Length; i++)
        {
            slots[i].image.color         = Color.clear;
            slots[i].stack.text          = "";
            slots[i].button.interactable = false;
        }
    }
}
