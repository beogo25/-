using System.Collections;
using System.Collections.Generic;
using UnityEngine;



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
public enum SceneName
{
    LOAD,
    NEW,
    MAIN
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
    HP_HEALTH,
    ANTIDOTE,
    ATK_UP,
    DEF_UP
}


//NPC의 UI 기능 분류
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

//상태이상
public enum StatusAilment
{
    ATTACK_UP = 1,
    DEFENCE_UP = 2,
    POISON = 4,
    BURN = 8
}

//공격 타입
public enum AttackType
{
    NORMAL,
    POISON,
    BURN
}
