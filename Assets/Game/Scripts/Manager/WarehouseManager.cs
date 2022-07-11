using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WarehouseManager : Singleton<WarehouseManager>
{
    public List<MaterialItem> materialItemList = new List<MaterialItem>();
    public List<Equipment> equipmentList = new List<Equipment>();
    public List<UseItem> useItemList = new List<UseItem>();
    
    public override void Awake()
    {
        base.Awake();
    }


    public void AddItem(Item input)
    {
        switch(input.itemType)
        {
            case ItemType.MATERIAL:
                MaterialAdd((MaterialItem)input);
                break;
            case ItemType.EQUIPMENT:
                EquipmentAdd((Equipment)input);
                break;
            case ItemType.USEITEM:
                UseItemAdd((UseItem)input);
                break;
            default:
                break;
        }
    }

    void MaterialAdd(MaterialItem input)
    {
        bool inputCheck = false;
        foreach(MaterialItem item in materialItemList)
        {
            if(item.itemName == input.itemName)
            {
                item.stack++;
                inputCheck = true;
                break;
            }
        }
        if(!inputCheck)
            materialItemList.Add(input);
    }
    void EquipmentAdd(Equipment input)
    {
        equipmentList.Add(input);
    }
    void UseItemAdd(UseItem input)
    {
        bool inputCheck = false;
        foreach (UseItem item in useItemList)
        {
            if (item.itemName == input.itemName)
            {
                item.stack++;
                inputCheck = true;
                break;
            }
        }
        if (!inputCheck)
            useItemList.Add(input);
    }
}
