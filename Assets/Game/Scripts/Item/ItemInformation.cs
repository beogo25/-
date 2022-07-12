using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInformation : MonoBehaviour
{
    private Item item;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI contents;
    public TextMeshProUGUI stack;
    public Image image;

    public GameObject outWarehouseButton;
    public GameObject outInventoryButton;

   

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
        if (WarehouseManager.instance.useItemList[targetNum].stack == 1)
            outWarehouseButton.SetActive(false);
        WarehouseManager.instance.MinusItem(targetNum,ItemType.USEITEM);
        InventoryManager.instance.AddItem(ItemListManager.instance.UseItemDic[Item.itemName]);
    }
    public void OutInventoryItem()
    {
        if (InventoryManager.instance.useItemList[targetNum].stack == 1)
            outInventoryButton.SetActive(false);
        InventoryManager.instance.MinusItem(targetNum, 1);
        WarehouseManager.instance.AddItem(ItemListManager.instance.UseItemDic[Item.itemName]);
    }
    private void OnEnable()
    {
        Item = null;
    }
} 
