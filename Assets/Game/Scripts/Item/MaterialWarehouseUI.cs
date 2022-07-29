using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialWarehouseUI : WarehouseUI
{
    public override void ItemInformationChange(int num)
    {
        itemInformation.Item = WarehouseManager.instance.materialItemList[num];
        itemInformation.ButtonSet();
        itemInformation.targetNum = num;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        if (GameManager.isJoyPadOn)
            GameManager.instance.eventSystem.SetSelectedGameObject(slots[0].transform.GetChild(2).transform.gameObject);
    }
    public override void Refresh()
    {
        int num = slots.Length;
        if (num > WarehouseManager.instance.materialItemList.Count)
            num = WarehouseManager.instance.materialItemList.Count;
        for (int i = 0; i < num; i++)
        {
            slots[i].image.color         = Color.white;
            slots[i].image.sprite        = WarehouseManager.instance.materialItemList[i].sprite;
            slots[i].stack.text          = WarehouseManager.instance.materialItemList[i].stack.ToString();
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
