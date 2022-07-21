using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemInformationSystem : MonoBehaviour
{
    private Item item;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI contents;
    public TextMeshProUGUI stack;
    public Image image;

    public GameObject outWarehouseButton;
    public GameObject outInventoryButton;
    public EventSystem eventSystem;
   

    public bool warehouseBool;
    public int targetNum;
    public Item Item
    {
        get { return item; }
        set
        {
            item = value;
            if(value != null)
            {
                itemName.text = item.itemName;
                contents.text = item.contents;
                image.sprite = item.sprite;
                stack.text = "";
                switch(item.itemType)
                {
                    case ItemType.USEITEM:
                        stack.text = "최대 : " + ((UseItem)item).maxStack.ToString() + "개";
                        break;
                    case ItemType.EQUIPMENT:
                        if (((EquipmentItem)item).equipmentType == EquipmentType.WEAPON)
                            stack.text = "공격력 : " + ((EquipmentItem)item).equipmentValue;
                        else
                            stack.text = "방어력 : " + ((EquipmentItem)item).equipmentValue;
                        break;
                    default:
                        break;
                }
                //아이템  타입에 따라 다른 행동
            }
        }
    }
    public bool WareHouseBool
    {
        get { return warehouseBool; }
        set
        {
            warehouseBool = value;
            //교환 UI 활성화
            if(warehouseBool)
            {
                outWarehouseButton.SetActive(true);
                outInventoryButton.SetActive(false);
            }
            else
            {
                outWarehouseButton.SetActive(false);
                outInventoryButton.SetActive(true);
            }
        }
    }

    public void OutWarehouseItem()
    {
        switch(item.itemType)
        {
            case ItemType.USEITEM:
                if(InventoryManager.instance.AddItem(DataManager.instance.useItemDic[Item.itemName]))
                {
                    if (WarehouseManager.instance.useItemList[targetNum].stack == 1)
                        outWarehouseButton.SetActive(false);
                    WarehouseManager.instance.MinusItem(targetNum, ItemType.USEITEM);
                }
                break;
            case ItemType.EQUIPMENT:
                outWarehouseButton.SetActive(false);
                InventoryManager.instance.Equip(((EquipmentItem)item));
                WarehouseManager.instance.MinusItem(targetNum, ItemType.EQUIPMENT);
                break;
            default:
                break;
        }
    }
    public void OutInventoryItem()
    {
        switch (item.itemType)
        {
            case ItemType.USEITEM:
                if (InventoryManager.instance.useItemList[targetNum].stack == 1)
                    outInventoryButton.SetActive(false);
                InventoryManager.instance.MinusItem(targetNum, 1);
                WarehouseManager.instance.AddItem(DataManager.instance.useItemDic[Item.itemName]);
                break;
            case ItemType.EQUIPMENT:
                outInventoryButton.SetActive(false);
                InventoryManager.instance.UnEquip(((EquipmentItem)item).equipmentType);
                break;
            default:
                break;
        }
    }
    private void OnEnable()
    {
        Item = null;
    }
} 
