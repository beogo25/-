using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�÷��̾� ���� 
enum PlayerDirection
{
    NORTH,
    NORTH_EAST,
    EAST,
    EAST_SOUTH,
    SOUTH,
    SOUTH_WEST,
    WEST,
    WEST_NORTH,
}

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
