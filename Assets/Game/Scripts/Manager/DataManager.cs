using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class MaterialData
{
    public List<string> itemname       = new List<string>();
    public List<string> contents       = new List<string>();
    public List<int>    value          = new List<int>();
    public List<int>    imageNum       = new List<int>();
}

[System.Serializable]
public class EquipmentData
{
    public List<string> itemname       = new List<string>();
    public List<string> contents       = new List<string>();
    public List<int>    value          = new List<int>();
    public List<int>    imageNum       = new List<int>();
    public List<int>    itemtype       = new List<int>();
    public List<int>    equipmentValue = new List<int>();
}

[System.Serializable]
public class UseItemData
{
    public List<string> itemname       = new List<string>();
    public List<string> contents       = new List<string>();
    public List<int>    value          = new List<int>();
    public List<int>    imageNum       = new List<int>();
    public List<int>    itemtype       = new List<int>();
    public List<int>    effectValue    = new List<int>();
    public List<int>    maxStack       = new List<int>();
    public List<int>    useItemType    = new List<int>();
}

[System.Serializable]
public class UseItemRecipeData
{
    public List<string> materialA = new List<string>();
    public List<string> materialB = new List<string>();
    public List<string> result    = new List<string>();
}

[System.Serializable]
public class EquipmentItemRecipeData
{
    public List<string> materialA = new List<string>();
    public List<int> numA = new List<int>();
    public List<string> materialB = new List<string>();
    public List<int> numB = new List<int>();
    public List<int> gold = new List<int>();
    public List<string> result = new List<string>();
}

public class DataManager : Singleton<DataManager>
{
    public Dictionary<string, MaterialItem> materialsDic = new Dictionary<string, MaterialItem>();
    public Dictionary<string, EquipmentItem> equipmentDic    = new Dictionary<string, EquipmentItem>();
    public Dictionary<string, UseItem> useItemDic        = new Dictionary<string, UseItem>();

    public List<UseItemRecipe> useItemRecipeList = new List<UseItemRecipe>();
    public List<EqiupmentItemRecipe> eqiupmentItemRecipeList = new List<EqiupmentItemRecipe>();

    public override void Awake()
    {
        base.Awake();
        LoadItemListData();
    }
    /*
    void SaveItemListData()
    {
        itemData  = Resources.LoadAll<ScriptableMaterialItem>("Item/MaterialItem");
        Data data = new Data();
        for (int i = 0; i < itemData.Length; i++)
        {
            data.itemname.Add(itemData[i].itemName);
            data.contents.Add(itemData[i].contents);
            data.value.Add(itemData[i].value);
            data.imageNum.Add(int.Parse(itemData[i].sprite.name));
        }
        File.WriteAllText(Application.dataPath + "/Game/Resources/Json/MaterialItemJson.json", JsonUtility.ToJson(data));
    }*/

    public void LoadItemListData()
    {
        //마테리얼 아이템
        string materialData               = Resources.Load<TextAsset>("Json/MaterialItemJson").text;

        if (materialData != null)
        {
            MaterialData loadData         = JsonUtility.FromJson<MaterialData>(materialData);
            for (int i = 0; i < loadData.itemname.Count; i++)
            {
                MaterialItem materialItem = new MaterialItem();
                materialItem.contents     = loadData.contents[i];
                materialItem.value        = loadData.value[i];
                materialItem.itemName     = loadData.itemname[i];
                materialItem.itemType     = ItemType.MATERIAL;
                materialItem.itemNumber   = loadData.imageNum[i];
                materialItem.sprite       = Resources.Load<Texture2D>("Image/Material/" + loadData.imageNum[i]).ToSprite();
                materialsDic.Add(materialItem.itemName, materialItem);
            }
        }

        //장비 아이템
        string equipmentData              = Resources.Load<TextAsset>("Json/Equipment").text;
        if (equipmentData != null)
        {
            EquipmentData loadData        = JsonUtility.FromJson<EquipmentData>(equipmentData);
            for (int i = 0; i < loadData.itemname.Count; i++)
            {
                EquipmentItem equipment   = new EquipmentItem();
                equipment.itemName        = loadData.itemname[i];
                equipment.contents        = loadData.contents[i];
                equipment.value           = loadData.value[i];
                equipment.equipmentType   = (EquipmentType)loadData.itemtype[i];
                equipment.itemType        = ItemType.EQUIPMENT;
                equipment.itemNumber      = loadData.imageNum[i];
                equipment.equipmentValue  = loadData.equipmentValue[i];
                equipment.sprite          = Resources.Load<Texture2D>("Image/Equipment/" + loadData.imageNum[i]).ToSprite();
                equipmentDic.Add(equipment.itemName, equipment);
            }
        }

        //사용 아이템
        string UseItemData                = Resources.Load<TextAsset>("Json/UseItemJson").text;
        if (UseItemData != null)
        {
            UseItemData loadData          = JsonUtility.FromJson<UseItemData>(UseItemData);
            for (int i = 0; i < loadData.itemname.Count; i++)
            {
                UseItem useItem           = new UseItem();
                useItem.itemName          = loadData.itemname[i];
                useItem.contents          = loadData.contents[i];
                useItem.value             = loadData.value[i];
                useItem.effectValue       = loadData.effectValue[i];
                useItem.maxStack          = loadData.maxStack[i];    
                useItem.itemType          = ItemType.USEITEM;
                useItem.useItemType       = (UseItemType)loadData.useItemType[i];
                useItem.itemNumber        = loadData.imageNum[i];
                useItem.sprite            = Resources.Load<Texture2D>("Image/UseItem/" + loadData.imageNum[i]).ToSprite();
                useItemDic.Add(useItem.itemName, useItem);
            }
        }

        //사용아이템 레시피
        string UseItemRecipeData = Resources.Load<TextAsset>("Json/UseItemRecipeJson").text;
        if (UseItemRecipeData != null)
        {
            UseItemRecipeData loadData = JsonUtility.FromJson<UseItemRecipeData>(UseItemRecipeData);
            for (int i = 0; i < loadData.materialA.Count; i++)
            {
                UseItemRecipe useItemRecipe = new UseItemRecipe();
                useItemRecipe.result = loadData.result[i];
                useItemRecipe.materialA = loadData.materialA[i];
                useItemRecipe.materialB = loadData.materialB[i];
                useItemRecipeList.Add(useItemRecipe);
            }
        }

        //장비아이템 레시피
        string EquipmentItemRecipeData = Resources.Load<TextAsset>("Json/EquipmentItemRecipeJson").text;
        if (EquipmentItemRecipeData != null)
        {
            EquipmentItemRecipeData loadData = JsonUtility.FromJson<EquipmentItemRecipeData>(EquipmentItemRecipeData);
            for (int i = 0; i < loadData.materialA.Count; i++)
            {
                EqiupmentItemRecipe eqiupmentItemRecipe = new EqiupmentItemRecipe();
                eqiupmentItemRecipe.result = loadData.result[i];
                eqiupmentItemRecipe.materialA = loadData.materialA[i];
                eqiupmentItemRecipe.materialB = loadData.materialB[i];
                eqiupmentItemRecipe.numA = loadData.numA[i];
                eqiupmentItemRecipe.numB = loadData.numB[i];
                eqiupmentItemRecipe.gold = loadData.gold[i];
                eqiupmentItemRecipeList.Add(eqiupmentItemRecipe);
            }
        }
    }


}
