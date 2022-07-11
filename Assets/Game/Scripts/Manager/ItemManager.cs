using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class MaterialData
{
    public List<string> itemname = new List<string>();
    public List<string> contents = new List<string>();
    public List<int> value       = new List<int>();
    public List<int> imageNum    = new List<int>();   
}

[System.Serializable]
public class EquipmentData
{
    public List<string> itemname = new List<string>();
    public List<string> contents = new List<string>();
    public List<int> value       = new List<int>();
    public List<int> imageNum    = new List<int>();
    public List<int> itemtype    = new List<int>();
}

public class ItemManager : MonoBehaviour
{
    ScriptableMaterialItem[] itemData;
    static public Dictionary<string, MaterialItem> materialsDic = new Dictionary<string, MaterialItem>();
    static public Dictionary<string, Equipment> equipmentDic    = new Dictionary<string, Equipment>();

    private void Awake()
    {
    }
    void Start()
    {
        LoadItemListData();
    }
    /*
    void SaveItemListData()
    {
        itemData = Resources.LoadAll<ScriptableMaterialItem>("Item/MaterialItem");
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
        string materialData = Resources.Load<TextAsset>("Json/MaterialItemJson").text;
        //string materialData = File.ReadAllText(Application.dataPath + "/Game/Resources/Json/MaterialItemJson.json");

        if (materialData != null)
        {
            MaterialData loadData = JsonUtility.FromJson<MaterialData>(materialData);
            for (int i = 0; i < loadData.itemname.Count; i++)
            {
                MaterialItem materialItem = new MaterialItem();
                materialItem.contents     = loadData.contents[i];
                materialItem.value        = loadData.value[i];
                materialItem.itemName     = loadData.itemname[i];
                materialItem.itemType     = ItemType.MATERIAL;


                materialItem.sprite       = Resources.Load<Texture2D>("Image/Material/" + loadData.imageNum[i]).ToSprite();


                //Debug.LogFormat($"{materialItem.contents} {materialItem.value} {materialItem.itemName}");
                materialsDic.Add(materialItem.itemName, materialItem);
            }
        }

        //장비 아이템
        string equipmentData = Resources.Load<TextAsset>("Json/Equipment").text;
        Debug.Log(equipmentData);
        //string equipmentData = File.ReadAllText(Application.dataPath + "/Game/Resources/Json/Equipment.json");
        if (equipmentData != null)
        {
            EquipmentData loadData = JsonUtility.FromJson<EquipmentData>(equipmentData);
            for (int i = 0; i < loadData.itemname.Count; i++)
            {
                Equipment equipment     = new Equipment();
                equipment.itemName      = loadData.itemname[i];
                equipment.contents      = loadData.contents[i];
                equipment.value         = loadData.value[i];
                equipment.equipmentType = (EquipmentType)loadData.itemtype[i];
                equipment.itemType      = ItemType.EQUIPMENT;
                Debug.LogFormat($"{equipment.contents} {equipment.value} {equipment.itemName}");
                equipment.sprite        = Resources.Load<Texture2D>("Image/Equipment/" + loadData.imageNum[i]).ToSprite();
                equipmentDic.Add(equipment.itemName, equipment);
            }
        }
    }


}
