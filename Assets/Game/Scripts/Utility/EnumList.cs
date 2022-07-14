using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//�Է� Ű
public enum EnumKey
{
    FRONT,
    BACK,
    LEFT,
    RIGHT,
    PUNCH,
    POWERPUNCH
}

public enum ItemType
{
    MATERIAL,
    EQUIPMENT,
    USEITEM
}

//������
public enum EquipmentType
{
    WEAPON,
    HELM,
    CHEST,
    ARM,
    WAIST,
    LEG
}

//�Һ������ Ÿ��
public enum UseItemType
{
    HP_HEALTH
}


//NPC�� UI ��� �з�
public enum UIType
{
    EquipmentUI,
    UseItemUI,
    MaterialUI,
    UseItemConbinationUI
}
