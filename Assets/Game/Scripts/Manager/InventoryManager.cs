using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InventoryManager : Singleton<InventoryManager>
{
    public UseItem[] useItemList = new UseItem[24];
    private List<UseItem> tempList = new List<UseItem>();
    private int itemcount = 0;

    public EquipmentItem[] equipmentList = new EquipmentItem[6];


    public Inventory inventory;
    public EquipmentInventory equipmentInventory;
    public PlayerStatus status;
    public BattleItemSystem itemUI;

    public int ItemCount
    {
        get { return itemcount; }
        set { itemcount = value; }  
    }
    public override void Awake()
    {
        base.Awake();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            AddItem(ItemListManager.instance.useItemDic["비약"]);
            AddItem(ItemListManager.instance.useItemDic["해독제"]);
            AddItem(ItemListManager.instance.useItemDic["포션"]);
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            for (int i = 0; i < ItemCount; i++)
                Debug.Log(useItemList[i].itemName + useItemList[i].stack);
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            MinusItem(1, 1);
        }
    }

    public bool AddItem(UseItem target)
    {
        bool addCount = false;
        bool pass = false;
        for(int i = 0; i < itemcount; i++)
        {
            if(useItemList[i].itemName==target.itemName)
            {
                if(useItemList[i].stack== useItemList[i].maxStack)
                {
                    pass = true;
                    break;
                }
                else
                {
                    useItemList[i].stack++;
                    addCount = true;
                    break;
                }
            }
        }
        if(addCount == false && pass == false)
        {
            for(int i = 0; i < useItemList.Length; i++)
            {
                if(useItemList[i]==null)
                {
                    useItemList[i] = new UseItem(target);
                    useItemList[i].stack = 1;
                    addCount = true;
                    ItemCount++;
                    break;
                }
            }
        }
        if (inventory.gameObject.activeInHierarchy)
            inventory.Refresh();
        if (itemUI.gameObject.activeInHierarchy)
            itemUI.SelectNum = itemUI.SelectNum;
        return addCount;
    }

    public void MinusItem(int target, int num)
    {
        useItemList[target].stack -= num;
        if(useItemList[target].stack <= 0)
        {
            for(int i=target; i < ItemCount; i++)
            {
                if(i==23)
                {
                    useItemList[23] = null;
                    break;
                }
                useItemList[i] = useItemList[i + 1];
            }
            ItemCount--;
        }
        if (inventory.gameObject.activeInHierarchy)
            inventory.Refresh();
        if (itemUI.gameObject.activeInHierarchy)
            itemUI.SelectNum = itemUI.SelectNum;
    }

    public void SortItem(bool ascend = true)
    {
        tempList.Clear();
        for (int i = 0; i < ItemCount; i++)
            tempList.Add(useItemList[i]);
        if(ascend)
        {
            tempList.Sort((x, y) =>
            {
                if (x.itemNumber < y.itemNumber)
                    return -1;
                if (x.itemNumber > y.itemNumber)
                    return 1;
                return 0;
            });
        }
        else
        {
            tempList.Sort((x, y) =>
            {
                if (x.itemNumber > y.itemNumber)
                    return -1;
                if (x.itemNumber < y.itemNumber)
                    return 1;
                return 0;
            });
        }
        for (int i = 0; i < ItemCount; i++)
            useItemList[i] = (tempList[i]);
        if (inventory.gameObject.activeInHierarchy)
            inventory.Refresh();
        if (itemUI.gameObject.activeInHierarchy)
            itemUI.SelectNum = itemUI.SelectNum;
    }

    public void Equip(EquipmentItem target)
    {
        if(equipmentList[(int)target.equipmentType] == null)
        {
            equipmentList[(int)target.equipmentType] = target;
        }
        else
        {
            UnEquip(target.equipmentType);
            equipmentList[(int)target.equipmentType] = target;
        }
        if (target.equipmentType == EquipmentType.WEAPON)
            status.atk += target.equipmentValue;
        else
            status.def += target.equipmentValue;
        equipmentInventory.Refresh();
    }
    public void UnEquip(EquipmentType target)
    {
        if (target == EquipmentType.WEAPON)
            status.atk -= equipmentList[(int)target].equipmentValue;
        else
            status.def -= equipmentList[(int)target].equipmentValue;
        WarehouseManager.instance.AddItem(equipmentList[(int)target]);
        equipmentList[(int)target]=null;
        equipmentInventory.Refresh();
    }
}
