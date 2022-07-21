using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentWarehouseUI : WarehouseUI
{
    public Button equipmentInventorySlot;
    public override void ItemInformationChange(int num)
    {
        itemInformation.Item = WarehouseManager.instance.equipmentList[num];
        base.ItemInformationChange(num);
    }
    public override void Refresh()
    {
        if (equipmentInventorySlot.interactable == false)
            GameManager.instance.eventSystem.SetSelectedGameObject(slots[0].transform.GetChild(2).transform.gameObject);
        else
            GameManager.instance.eventSystem.SetSelectedGameObject(equipmentInventorySlot.transform.gameObject);



        GameManager.instance.eventSystem.SetSelectedGameObject(slots[0].transform.GetChild(2).transform.gameObject);
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
