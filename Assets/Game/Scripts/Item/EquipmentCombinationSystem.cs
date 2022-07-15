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
        for (int i = 0; i < JsonManager.instance.eqiupmentItemRecipeList.Count; i++)
        {
            CombiContentsUI temp = Instantiate(contentsPrifab, contents.transform).GetComponent<CombiContentsUI>();
            temp.image.sprite = JsonManager.instance.equipmentDic[JsonManager.instance.eqiupmentItemRecipeList[i].result].sprite;
            temp.textMeshProUGUI.text = JsonManager.instance.eqiupmentItemRecipeList[i].result;
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
        EquipmentItem equipmentItem = JsonManager.instance.equipmentDic[JsonManager.instance.eqiupmentItemRecipeList[num].result];
        MaterialItem materialItemA = JsonManager.instance.materialsDic[JsonManager.instance.eqiupmentItemRecipeList[num].materialA];
        MaterialItem materialItemB = JsonManager.instance.materialsDic[JsonManager.instance.eqiupmentItemRecipeList[num].materialB];
        int materialAnum = WarehouseManager.instance.FindItem(materialItemA);
        int materialBnum = WarehouseManager.instance.FindItem(materialItemB);

        resultImage.sprite = equipmentItem.sprite;
        materialAImage.sprite = materialItemA.sprite;
        materialBImage.sprite = materialItemB.sprite;

        resultName.text = equipmentItem.itemName;
        materialAName.text = materialItemA.itemName + JsonManager.instance.eqiupmentItemRecipeList[num].numA + " (" + materialAnum + ")";
        materialBName.text = materialItemB.itemName + JsonManager.instance.eqiupmentItemRecipeList[num].numB + " (" + materialBnum + ")";
        needGold.text = JsonManager.instance.eqiupmentItemRecipeList[num].gold.ToString();
        if (materialAnum >= JsonManager.instance.eqiupmentItemRecipeList[num].numA && materialBnum >= JsonManager.instance.eqiupmentItemRecipeList[num].numB && PlayerStatus.gold >= JsonManager.instance.eqiupmentItemRecipeList[num].gold)
            combinationButton.SetActive(true);
        else
            combinationButton.SetActive(false);
    }

    public void Combination()
    {
        WarehouseManager.instance.AddItem  (JsonManager.instance.equipmentDic[JsonManager.instance.eqiupmentItemRecipeList[target].result   ]                                                           );
        WarehouseManager.instance.MinusItem(JsonManager.instance.materialsDic[JsonManager.instance.eqiupmentItemRecipeList[target].materialA], JsonManager.instance.eqiupmentItemRecipeList[target].numA);
        WarehouseManager.instance.MinusItem(JsonManager.instance.materialsDic[JsonManager.instance.eqiupmentItemRecipeList[target].materialB], JsonManager.instance.eqiupmentItemRecipeList[target].numB);
        PlayerStatus.gold -= JsonManager.instance.eqiupmentItemRecipeList[target].gold;
        CombiRecipeView(target);
    }
}
