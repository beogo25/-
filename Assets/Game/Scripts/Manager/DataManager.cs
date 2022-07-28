using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;

public class DataManager : Singleton<DataManager>
{
    public Dictionary<string, MaterialItem>  materialsDic    = new Dictionary<string, MaterialItem>();
    public Dictionary<string, EquipmentItem> equipmentDic    = new Dictionary<string, EquipmentItem>();
    public Dictionary<string, UseItem>       useItemDic      = new Dictionary<string, UseItem>();
    public Dictionary<string, WeaponData>    weaponDataDic   = new Dictionary<string, WeaponData>();
    

    public List<UseItemRecipe>       useItemRecipeList       = new List<UseItemRecipe>();
    public List<EqiupmentItemRecipe> eqiupmentItemRecipeList = new List<EqiupmentItemRecipe>();
    public List<Quest>               questList               = new List<Quest>();

    public EventReference saveSound;

    public override void Awake()
    {
        base.Awake();
        LoadItemListData();
    }
<<<<<<< HEAD

=======
>>>>>>> master
    private void Start()
    {
        if (GameManager.instance.load)
            LoadData();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
            LoadData();
    }

    public void SaveData()
    {
        MainCanvas.instance.PlaySoundOneShot(saveSound.Path);
        GameSaveData saveData = new GameSaveData();
        for(int i = 0; i < WarehouseManager.instance.equipmentList.Count; i++)
        {
            saveData.equipmentWarehouseItem.Add(WarehouseManager.instance.equipmentList[i].itemName);
        }
        for(int i = 0; i < WarehouseManager.instance.useItemList.Count; i++)
        {
            saveData.useItemWarehouseItem.Add(WarehouseManager.instance.useItemList[i].itemName);
            saveData.useItemWarehouseItemNum.Add(WarehouseManager.instance.useItemList[i].stack);
        }
        for (int i = 0; i < WarehouseManager.instance.materialItemList.Count; i++)
        {
            saveData.materialWarehouseItem.Add(WarehouseManager.instance.materialItemList[i].itemName);
            saveData.materialWarehouseItemNum.Add(WarehouseManager.instance.materialItemList[i].stack);
        }
        for (int i = 0; i < InventoryManager.instance.equipmentList.Length; i++)
        {
            if(InventoryManager.instance.equipmentList[i] !=null)
                saveData.equipInventoryItem.Add(InventoryManager.instance.equipmentList[i].itemName);
        }
        for (int i = 0; i < InventoryManager.instance.useItemList.Length; i++)
        {
            if (InventoryManager.instance.useItemList[i] != null)
            {
                saveData.inventoryItem.Add(InventoryManager.instance.useItemList[i].itemName);
                saveData.inventoryItemNum.Add(InventoryManager.instance.useItemList[i].stack);
            }
        }
        saveData.gold = PlayerStatus.gold;
        File.WriteAllText(Application.dataPath + "/Game/Resources/Json/SaveJson.json", JsonUtility.ToJson(saveData));
    }

    public void LoadData()
    {
        string saveData = Resources.Load<TextAsset>("Json/SaveJson").text;
        if(saveData != null)
        {
            GameSaveData loadData = JsonUtility.FromJson<GameSaveData>(saveData);
            for (int i = 0; i < loadData.equipmentWarehouseItem.Count; i++)
                WarehouseManager.instance.AddItem(equipmentDic[loadData.equipmentWarehouseItem[i]]);
            for (int i = 0; i < loadData.useItemWarehouseItem.Count; i++)
            {
                for(int j = 0; j < loadData.useItemWarehouseItemNum[i];j++)
                    WarehouseManager.instance.AddItem(useItemDic[loadData.useItemWarehouseItem[i]]);
            }
            for (int i = 0; i < loadData.materialWarehouseItem.Count; i++)
            {
                for (int j = 0; j < loadData.materialWarehouseItemNum[i]; j++)
                    WarehouseManager.instance.AddItem(materialsDic[loadData.materialWarehouseItem[i]]);
            }
            for (int i = 0; i < loadData.equipInventoryItem.Count; i++)
            {
                InventoryManager.instance.Equip(equipmentDic[loadData.equipInventoryItem[i]]);
            }
            for (int i = 0; i < loadData.inventoryItem.Count; i++)
            {
                for (int j = 0; j < loadData.inventoryItemNum[i]; j++)
                    InventoryManager.instance.AddItem(useItemDic[loadData.inventoryItem[i]]);
            }
        }
    }

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
        string useItemData                = Resources.Load<TextAsset>("Json/UseItemJson").text;
        if (useItemData != null)
        {
            UseItemData loadData          = JsonUtility.FromJson<UseItemData>(useItemData);
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
        string useItemRecipeData = Resources.Load<TextAsset>("Json/UseItemRecipeJson").text;
        if (useItemRecipeData != null)
        {
            UseItemRecipeData loadData = JsonUtility.FromJson<UseItemRecipeData>(useItemRecipeData);
            for (int i = 0; i < loadData.materialA.Count; i++)
            {
                UseItemRecipe useItemRecipe = new UseItemRecipe();
                useItemRecipe.result        = loadData.result[i];
                useItemRecipe.materialA     = loadData.materialA[i];
                useItemRecipe.materialB     = loadData.materialB[i];
                useItemRecipeList.Add(useItemRecipe);
            }
        }

        //장비아이템 레시피
        string equipmentItemRecipeData = Resources.Load<TextAsset>("Json/EquipmentItemRecipeJson").text;
        if (equipmentItemRecipeData != null)
        {
            EquipmentItemRecipeData loadData = JsonUtility.FromJson<EquipmentItemRecipeData>(equipmentItemRecipeData);
            for (int i = 0; i < loadData.materialA.Count; i++)
            {
                EqiupmentItemRecipe eqiupmentItemRecipe = new EqiupmentItemRecipe();
                eqiupmentItemRecipe.result              = loadData.result[i];
                eqiupmentItemRecipe.materialA           = loadData.materialA[i];
                eqiupmentItemRecipe.materialB           = loadData.materialB[i];
                eqiupmentItemRecipe.numA                = loadData.numA[i];
                eqiupmentItemRecipe.numB                = loadData.numB[i];
                eqiupmentItemRecipe.gold                = loadData.gold[i];
                eqiupmentItemRecipeList.Add(eqiupmentItemRecipe);
            }
        }

        //퀘스트 리스트
        string questData = Resources.Load<TextAsset>("Json/QuestJson").text;
        if (questData != null)
        {
            QuestData loadData = JsonUtility.FromJson<QuestData>(questData);
            for (int i = 0; i < loadData.questName.Count; i++)
            {
                Quest quest           = new Quest();
                quest.questName       = loadData.questName[i];
                quest.questContents   = loadData.questContents[i];
                quest.questDifficulty = loadData.questDifficulty[i];
                quest.clearGold       = loadData.clearGold[i];
                quest.target          = loadData.target[i];
                quest.targetNum       = loadData.targetNum[i];
                quest.collectionQuest = loadData.collectionQuest[i];
                quest.questNum        = i;
                questList.Add(quest);
            }
        }

        //장비 렌더러 정보
        WeaponData[] weaponDatas;
        weaponDatas = Resources.LoadAll<WeaponData>("WeaponData");
        if(weaponDatas != null)
        {
            for (int i = 0; i < weaponDatas.Length; i++)
            {
                weaponDataDic.Add(weaponDatas[i].weaponName, weaponDatas[i]);
            }
        }
    }

    [System.Serializable]
    public class MaterialData
    {
        public List<string> itemname = new List<string>();
        public List<string> contents = new List<string>();
        public List<int>    value    = new List<int>();
        public List<int>    imageNum = new List<int>();
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
        public List<string> itemname    = new List<string>();
        public List<string> contents    = new List<string>();
        public List<int>    value       = new List<int>();
        public List<int>    imageNum    = new List<int>();
        public List<int>    itemtype    = new List<int>();
        public List<int>    effectValue = new List<int>();
        public List<int>    maxStack    = new List<int>();
        public List<int>    useItemType = new List<int>();
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
        public List<int> numA         = new List<int>();
        public List<string> materialB = new List<string>();
        public List<int> numB         = new List<int>();
        public List<int> gold         = new List<int>();
        public List<string> result    = new List<string>();
    }

    [System.Serializable]
    public class QuestData
    {
        public List<string> target          = new List<string>();
        public List<int>    targetNum       = new List<int>();
        public List<int>    clearGold       = new List<int>();
        public List<string> questName       = new List<string>();
        public List<string> questContents   = new List<string>();
        public List<int>    questDifficulty = new List<int>();
        public List<bool>   collectionQuest = new List<bool>();
    }

    [System.Serializable]
    public class GameSaveData
    {
        public List<string> equipmentWarehouseItem   = new List<string>();
                                                     
        public List<string> useItemWarehouseItem     = new List<string>();
        public List<int>    useItemWarehouseItemNum  = new List<int>();

        public List<string> materialWarehouseItem    = new List<string>();
        public List<int>    materialWarehouseItemNum = new List<int>();

        public List<string> equipInventoryItem       = new List<string>();
                                                     
        public List<string> inventoryItem            = new List<string>();
        public List<int>    inventoryItemNum         = new List<int>();
        public int gold;
    }
}
