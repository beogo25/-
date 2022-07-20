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
    HP_HEALTH,
    ANTIDOTE,
    ATK_UP,
    DEF_UP
}


//NPC�� UI ��� �з�
public enum UIType
{
    EQIUPMENT_WAREHOUSE_UI,
    USEITEM_WAREHOUSE_UI,
    MATERIAL_WAREHOUSE_UI,
    USEITEM_COMBINATION_UI,
    EQIUPMENT_COMBINATION_UI,
    SHOP_UI,
    QUEST_UI
}
