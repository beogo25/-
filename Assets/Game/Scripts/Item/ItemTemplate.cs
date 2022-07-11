using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public string itemName;
    public string contents;
    public int value;
    public Sprite sprite;
    public ItemType itemType;
}

public class MaterialItem : Item
{
    public int stack=1;
}

public class UseItem : Item
{
    public UseItemType useItemType;
    public int effectValue;
    public int maxStack;
    public int stack=1;
}

public class Equipment : Item
{
    public EquipmentType equipmentType;
}

