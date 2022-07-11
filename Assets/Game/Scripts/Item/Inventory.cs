using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : Warehouse
{
    private void OnEnable()
    {
        int num = slots.Length;
        for (int i = 0; i < slots.Length; i++)
        {
            if(InventoryManager.instance.useItemList[i]==null)
            { 
                slots[i].gameObject.SetActive(false);
            }
            else
            {
                slots[i].image.sprite = InventoryManager.instance.useItemList[i].sprite;
                slots[i].stack.text = InventoryManager.instance.useItemList[i].stack.ToString();
                slots[i].gameObject.SetActive(true);
            }
        }
    }

    public override void ItemInformationChange(int num)
    {
        Debug.Log(num);
        itemInformation.image.sprite = InventoryManager.instance.useItemList[num].sprite;
        itemInformation.itemName.text = InventoryManager.instance.useItemList[num].itemName;
        itemInformation.stack.text = InventoryManager.instance.useItemList[num].stack.ToString()+" / "+ InventoryManager.instance.useItemList[num].maxStack.ToString();
        itemInformation.contents.text = InventoryManager.instance.useItemList[num].contents;
    }
}
