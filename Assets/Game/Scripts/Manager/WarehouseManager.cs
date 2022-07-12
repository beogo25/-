using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class WarehouseManager : Singleton<WarehouseManager>
{
    public List<MaterialItem> materialItemList = new List<MaterialItem>();
    public List<EquipmentItem> equipmentList = new List<EquipmentItem>();
    public List<UseItem> useItemList = new List<UseItem>();

    public UseItemWarehouse useItemWarehouse;
    public EquipmentWarehouse equipmentWarehouse;
    public MaterialWarehouse materialWarehouse;
    
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
                EquipmentAdd((EquipmentItem)input);
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
        {
            input.stack = 1;
            materialItemList.Add(input);
        }
    }
    void EquipmentAdd(EquipmentItem input)
    {
        equipmentList.Add(input);
        if (equipmentWarehouse.gameObject.activeInHierarchy)
            equipmentWarehouse.Refresh();
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
        {
            input.stack = 1;
            useItemList.Add(new UseItem(input));
        }
        if(useItemWarehouse.gameObject.activeInHierarchy)
            useItemWarehouse.Refresh();
    }
    
    public void MinusItem(int target,ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.MATERIAL:
                break;
            case ItemType.EQUIPMENT:
                equipmentList.RemoveAt(target);
                equipmentWarehouse.Refresh();
                break;
            case ItemType.USEITEM:
                useItemList[target].stack--;
                if(useItemList[target].stack <= 0)
                    useItemList.RemoveAt(target);
                useItemWarehouse.Refresh();
                break;
            default:
                break;
        }
    }
}
