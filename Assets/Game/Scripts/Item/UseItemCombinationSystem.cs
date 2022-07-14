using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UseItemCombinationSystem : MonoBehaviour
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
        UseItem useitem = ItemListManager.instance.useItemDic[ItemListManager.instance.useItemRecipeList[num].result];
        MaterialItem materialItemA = ItemListManager.instance.materialsDic[ItemListManager.instance.useItemRecipeList[num].materialA];
        MaterialItem materialItemB = ItemListManager.instance.materialsDic[ItemListManager.instance.useItemRecipeList[num].materialB];
        int resultNum = WarehouseManager.instance.FindItem(useitem);
        int materialAnum = WarehouseManager.instance.FindItem(materialItemA);
        int materialBnum = WarehouseManager.instance.FindItem(materialItemB);

        resultImage.sprite = useitem.sprite;
        materialAImage.sprite = materialItemA.sprite;
        materialBImage.sprite = materialItemB.sprite;

        resultName.text = useitem.itemName + "(" + resultNum + ")";
        materialAName.text = materialItemA.itemName + "1 (" + materialAnum + ")";
        materialBName.text = materialItemB.itemName + "1 (" + materialBnum + ")";
        if(materialAnum>0 && materialBnum>0)
            combinationButton.SetActive(true);
        else
            combinationButton.SetActive(false);
    }

    public void Combination()
    {
        WarehouseManager.instance.AddItem(ItemListManager.instance.useItemDic[ItemListManager.instance.useItemRecipeList[target].result]);
        WarehouseManager.instance.MinusItem(ItemListManager.instance.materialsDic[ItemListManager.instance.useItemRecipeList[target].materialA],1);
        WarehouseManager.instance.MinusItem(ItemListManager.instance.materialsDic[ItemListManager.instance.useItemRecipeList[target].materialB],1);
        CombiRecipeView(target);
    }
}
