using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//플레이어 방향 
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

//입력 키
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

//장비부위
public enum EquipmentType
{
    WEAPON,
    HELM,
    CHEST,
    ARM,
    WAIST,
    LEG
}

//소비아이템 타입
public enum UseItemType
{
    HP_HEALTH
}
