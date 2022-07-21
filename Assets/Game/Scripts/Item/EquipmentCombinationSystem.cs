using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentCombinationSystem : MonoBehaviour
{
    public GameObject contents;
    public GameObject contentsPrifab;
    private RectTransform contentsRectTransform;

    public Image resultImage;
    public Image materialAImage;
    public Image materialBImage;
    public TextMeshProUGUI resultContents;
    public TextMeshProUGUI materialAName;
    public TextMeshProUGUI materialBName;
    public TextMeshProUGUI needGold;
    public TextMeshProUGUI playerGold;

    public GameObject combinationButton;
    public MeshFilter weaponMesh;
    public MeshRenderer weaponRenderer;

    private int target;
    private void OnEnable()
    {
        combinationButton.SetActive(false);
    }
    private void Awake()
    {
        contentsRectTransform = contents.GetComponent<RectTransform>();
    }
    private void Start()
    {
        for (int i = 0; i < DataManager.instance.eqiupmentItemRecipeList.Count; i++)
        {
            CombiContentsUI temp = Instantiate(contentsPrifab, contents.transform).GetComponent<CombiContentsUI>();
            temp.image.sprite = DataManager.instance.equipmentDic[DataManager.instance.eqiupmentItemRecipeList[i].result].sprite;
            temp.textMeshProUGUI.text = DataManager.instance.eqiupmentItemRecipeList[i].result;
            int tempint = i;
            temp.gameObject.GetComponent<Button>().onClick.AddListener(() => { CombiRecipeView(tempint); });
            contentsRectTransform.sizeDelta = new Vector2(contentsRectTransform.sizeDelta.x, contentsRectTransform.sizeDelta.y + 50);
        }
        playerGold.text = "보유 금액 : " + PlayerStatus.gold.ToString();
    }

    public void CombiRecipeView(int num)
    {
        target = num;
        playerGold.text ="보유 금액 : " + PlayerStatus.gold.ToString();
        EquipmentItem equipmentItem = DataManager.instance.equipmentDic[DataManager.instance.eqiupmentItemRecipeList[num].result];
        MaterialItem materialItemA = DataManager.instance.materialsDic[DataManager.instance.eqiupmentItemRecipeList[num].materialA];
        MaterialItem materialItemB = DataManager.instance.materialsDic[DataManager.instance.eqiupmentItemRecipeList[num].materialB];
        int materialAnum = WarehouseManager.instance.FindItem(materialItemA);
        int materialBnum = WarehouseManager.instance.FindItem(materialItemB);

        resultImage.sprite = equipmentItem.sprite;
        materialAImage.sprite = materialItemA.sprite;
        materialBImage.sprite = materialItemB.sprite;

        if(equipmentItem.equipmentType == EquipmentType.WEAPON)
            resultContents.text = "공격력 : "+equipmentItem.equipmentValue;
        else
            resultContents.text = "방어력 : " + equipmentItem.equipmentValue;
        materialAName.text = materialItemA.itemName + DataManager.instance.eqiupmentItemRecipeList[num].numA + " (" + materialAnum + ")";
        materialBName.text = materialItemB.itemName + DataManager.instance.eqiupmentItemRecipeList[num].numB + " (" + materialBnum + ")";
        needGold.text = DataManager.instance.eqiupmentItemRecipeList[num].gold.ToString()+" 골드";
        if (materialAnum >= DataManager.instance.eqiupmentItemRecipeList[num].numA && materialBnum >= DataManager.instance.eqiupmentItemRecipeList[num].numB && PlayerStatus.gold >= DataManager.instance.eqiupmentItemRecipeList[num].gold)
            combinationButton.SetActive(true);
        else
            combinationButton.SetActive(false);
        if(equipmentItem.equipmentType==EquipmentType.WEAPON)
        {
            weaponMesh.mesh = DataManager.instance.weaponDataDic[equipmentItem.itemName].weaponMesh;
            weaponRenderer.materials[0] = DataManager.instance.weaponDataDic[equipmentItem.itemName].weaponMaterial;
        }
        else
        {
            weaponMesh.mesh = null;
        }
    }

    public void Combination()
    {
        WarehouseManager.instance.itemDelegate(DataManager.instance.equipmentDic[DataManager.instance.eqiupmentItemRecipeList[target].result   ]                                                           );
        WarehouseManager.instance.MinusItem(DataManager.instance.materialsDic[DataManager.instance.eqiupmentItemRecipeList[target].materialA], DataManager.instance.eqiupmentItemRecipeList[target].numA);
        WarehouseManager.instance.MinusItem(DataManager.instance.materialsDic[DataManager.instance.eqiupmentItemRecipeList[target].materialB], DataManager.instance.eqiupmentItemRecipeList[target].numB);
        PlayerStatus.gold -= DataManager.instance.eqiupmentItemRecipeList[target].gold;
        CombiRecipeView(target);
    }
}
