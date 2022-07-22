using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInformationSystem : MonoBehaviour
{
    private Item            item;
    public  TextMeshProUGUI itemName;
    public  TextMeshProUGUI contents;
    public  TextMeshProUGUI stack;
    public  Image           image;

    public  GameObject      outWarehouseButton;
    public  GameObject      outInventoryButton;
    public  GameObject      afterItemWareHouseButton;
    public  GameObject      afterEquipmentWareHouseButton;
    public  GameObject      afterItemInventoryButton;
    public  GameObject      afterEquipmentInventoryButton;
   

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
                image.sprite  = item.sprite;
                image.color   = Color.white;
                stack.text    = "";
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
            else
            {
                itemName.text = "";
                contents.text = "";
                stack.text    = "";
                image.color   = Color.clear;
            }
        }
    }

    public void ButtonSet()
    {
        outWarehouseButton.SetActive(false);
        outInventoryButton.SetActive(false);
    }
    public void ButtonSet(bool warehouseBool)
    {
        if (warehouseBool)
        {
            Debug.Log("true");
            outWarehouseButton.SetActive(true);
            GameManager.instance.eventSystem.SetSelectedGameObject(outWarehouseButton);
            outInventoryButton.SetActive(false);
        }
        else
        {
            Debug.Log("false");
            outWarehouseButton.SetActive(false);
            outInventoryButton.SetActive(true);
            GameManager.instance.eventSystem.SetSelectedGameObject(outInventoryButton);
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
                    {
                        outWarehouseButton.SetActive(false);
                        if(afterItemInventoryButton != null)
                            GameManager.instance.eventSystem.SetSelectedGameObject(afterItemInventoryButton);
                        else
                        {
                            GameManager.instance.eventSystem.SetSelectedGameObject(afterItemWareHouseButton);
                            Debug.Log("체크");
                        }
                    }
                    WarehouseManager.instance.MinusItem(targetNum, ItemType.USEITEM);
                }
                break;
            case ItemType.EQUIPMENT:
                outWarehouseButton.SetActive(false);
                GameManager.instance.eventSystem.SetSelectedGameObject(afterEquipmentWareHouseButton);
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
                {
                    outInventoryButton.SetActive(false);
                    if (afterItemWareHouseButton != null)
                        GameManager.instance.eventSystem.SetSelectedGameObject(afterItemWareHouseButton);
                    else
                        GameManager.instance.eventSystem.SetSelectedGameObject(afterItemInventoryButton);
                }
                InventoryManager.instance.MinusItem(targetNum, 1);
                WarehouseManager.instance.AddItem(DataManager.instance.useItemDic[Item.itemName]);
                break;
            case ItemType.EQUIPMENT:
                outInventoryButton.SetActive(false);
                GameManager.instance.eventSystem.SetSelectedGameObject(afterEquipmentInventoryButton);
                InventoryManager.instance.UnEquip(((EquipmentItem)item).equipmentType);
                break;
            default:
                break;
        }
    }
    private void OnEnable()
    {
        Item = null;
        ButtonSet();
    }
} 
