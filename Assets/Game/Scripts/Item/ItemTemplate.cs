using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string   itemName;
    public string   contents;
    public int      value;
    public Sprite   sprite;
    public ItemType itemType;
    public int      itemNumber;
}

public class MaterialItem : Item
{
    public int stack = 1;
    public MaterialItem()
    {

    }
    public MaterialItem(MaterialItem materialItem)
    {
        this.itemName = materialItem.itemName;
        this.contents = materialItem.contents;
        this.value = materialItem.value;
        this.sprite = materialItem.sprite;
        this.itemType = materialItem.itemType;
        this.itemNumber = materialItem.itemNumber;
        this.stack = materialItem.stack;
    }
}
public class UseItem : Item
{
    public UseItemType useItemType;
    public int         effectValue;
    public int         maxStack;
    public int         stack = 1;
    public UseItem()
    {

    }
    public UseItem(UseItem useItem)
    {
        this.itemName = useItem.itemName;
        this.contents = useItem.contents;
        this.value = useItem.value;
        this.sprite = useItem.sprite;
        this.itemType = useItem.itemType;
        this.itemNumber = useItem.itemNumber;
        this.stack = useItem.stack;
        this.useItemType = useItem.useItemType;
        this.effectValue = useItem.effectValue;
        this.maxStack = useItem.maxStack;   
    }
}
public class Equipment : Item
{
    public EquipmentType equipmentType;
}

