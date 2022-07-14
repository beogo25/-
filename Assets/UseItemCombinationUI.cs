using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UseItemCombinationUI : MonoBehaviour
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
        for(int i = 0; i < ItemListManager.instance.useItemRecipeList.Count; i++)
        {
            CombiContentsUI temp = Instantiate(contentsPrifab, contents.transform).GetComponent<CombiContentsUI>();
            temp.image.sprite = ItemListManager.instance.useItemDic[ItemListManager.instance.useItemRecipeList[i].result].sprite;
            temp.textMeshProUGUI.text = ItemListManager.instance.useItemRecipeList[i].result;
            int tempint = i;
            temp.gameObject.GetComponent<Button>().onClick.AddListener(() => { CombiRecipeView(tempint); });
            contentsRectTransform.sizeDelta = new Vector2(contentsRectTransform.sizeDelta.x, contentsRectTransform.sizeDelta.y + 50);
        }
    }

    public void CombiRecipeView(int num)
    {
        target = num;   
        int resultNum = WarehouseManager.instance.FindItem(ItemListManager.instance.useItemDic[ItemListManager.instance.useItemRecipeList[num].result]);
        int materialAnum = WarehouseManager.instance.FindItem(ItemListManager.instance.materialsDic[ItemListManager.instance.useItemRecipeList[num].materialA]);
        int materialBnum = WarehouseManager.instance.FindItem(ItemListManager.instance.materialsDic[ItemListManager.instance.useItemRecipeList[num].materialB]);

        resultImage.sprite = ItemListManager.instance.useItemDic[ItemListManager.instance.useItemRecipeList[num].result].sprite;
        materialAImage.sprite = ItemListManager.instance.materialsDic[ItemListManager.instance.useItemRecipeList[num].materialA].sprite;
        materialBImage.sprite = ItemListManager.instance.materialsDic[ItemListManager.instance.useItemRecipeList[num].materialB].sprite;

        resultName.text = ItemListManager.instance.useItemRecipeList[num].result + "(" + resultNum + ")";
        materialAName.text = ItemListManager.instance.useItemRecipeList[num].materialA+"(" + materialAnum + ")";
        materialBName.text = ItemListManager.instance.useItemRecipeList[num].materialB + "(" + materialBnum + ")";
        if(materialAnum>0 && materialBnum>0)
            combinationButton.SetActive(true);
        else
            combinationButton.SetActive(false);
    }

    public void Combination()
    {
        WarehouseManager.instance.AddItem(ItemListManager.instance.useItemDic[ItemListManager.instance.useItemRecipeList[target].result]);
        WarehouseManager.instance.MinusItem(ItemListManager.instance.materialsDic[ItemListManager.instance.useItemRecipeList[target].materialA]);
        WarehouseManager.instance.MinusItem(ItemListManager.instance.materialsDic[ItemListManager.instance.useItemRecipeList[target].materialB]);
        CombiRecipeView(target);
    }
}
