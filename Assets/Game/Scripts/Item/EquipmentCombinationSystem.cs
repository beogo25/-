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
    public TextMeshProUGUI resultName;
    public TextMeshProUGUI materialAName;
    public TextMeshProUGUI materialBName;
    public TextMeshProUGUI needGold;
    public TextMeshProUGUI playerGold;

    public GameObject combinationButton;

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
        for (int i = 0; i < ItemListManager.instance.eqiupmentItemRecipeList.Count; i++)
        {
            CombiContentsUI temp = Instantiate(contentsPrifab, contents.transform).GetComponent<CombiContentsUI>();
            temp.image.sprite = ItemListManager.instance.equipmentDic[ItemListManager.instance.eqiupmentItemRecipeList[i].result].sprite;
            temp.textMeshProUGUI.text = ItemListManager.instance.eqiupmentItemRecipeList[i].result;
            int tempint = i;
            temp.gameObject.GetComponent<Button>().onClick.AddListener(() => { CombiRecipeView(tempint); });
            contentsRectTransform.sizeDelta = new Vector2(contentsRectTransform.sizeDelta.x, contentsRectTransform.sizeDelta.y + 50);
        }
    }

    public void CombiRecipeView(int num)
    {
        target = num;
        playerGold.text = PlayerStatus.gold.ToString();
        EquipmentItem equipmentItem = ItemListManager.instance.equipmentDic[ItemListManager.instance.eqiupmentItemRecipeList[num].result];
        MaterialItem materialItemA = ItemListManager.instance.materialsDic[ItemListManager.instance.eqiupmentItemRecipeList[num].materialA];
        MaterialItem materialItemB = ItemListManager.instance.materialsDic[ItemListManager.instance.eqiupmentItemRecipeList[num].materialB];
        int materialAnum = WarehouseManager.instance.FindItem(materialItemA);
        int materialBnum = WarehouseManager.instance.FindItem(materialItemB);

        resultImage.sprite = equipmentItem.sprite;
        materialAImage.sprite = materialItemA.sprite;
        materialBImage.sprite = materialItemB.sprite;

        resultName.text = equipmentItem.itemName;
        materialAName.text = materialItemA.itemName + ItemListManager.instance.eqiupmentItemRecipeList[num].numA + " (" + materialAnum + ")";
        materialBName.text = materialItemB.itemName + ItemListManager.instance.eqiupmentItemRecipeList[num].numB + " (" + materialBnum + ")";
        needGold.text = ItemListManager.instance.eqiupmentItemRecipeList[num].gold.ToString();
        if (materialAnum >= ItemListManager.instance.eqiupmentItemRecipeList[num].numA && materialBnum >= ItemListManager.instance.eqiupmentItemRecipeList[num].numB && PlayerStatus.gold >= ItemListManager.instance.eqiupmentItemRecipeList[num].gold)
            combinationButton.SetActive(true);
        else
            combinationButton.SetActive(false);
    }

    public void Combination()
    {
        WarehouseManager.instance.AddItem(ItemListManager.instance.equipmentDic[ItemListManager.instance.eqiupmentItemRecipeList[target].result]);
        WarehouseManager.instance.MinusItem(ItemListManager.instance.materialsDic[ItemListManager.instance.eqiupmentItemRecipeList[target].materialA], ItemListManager.instance.eqiupmentItemRecipeList[target].numA);
        WarehouseManager.instance.MinusItem(ItemListManager.instance.materialsDic[ItemListManager.instance.eqiupmentItemRecipeList[target].materialB], ItemListManager.instance.eqiupmentItemRecipeList[target].numB);
        PlayerStatus.gold -= ItemListManager.instance.eqiupmentItemRecipeList[target].gold;
        CombiRecipeView(target);
    }
}
