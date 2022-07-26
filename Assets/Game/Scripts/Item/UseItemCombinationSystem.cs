using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UseItemCombinationSystem : MonoBehaviour
{
    public  GameObject      contents;
    public  GameObject      contentsPrefab;
    private RectTransform   contentsRectTransform;

    public  Image           resultImage;
    public  Image           materialAImage;
    public  Image           materialBImage;
    public  TextMeshProUGUI resultContents;
    public  TextMeshProUGUI materialAName;
    public  TextMeshProUGUI materialBName;
            
    public  GameObject      combinationButton;
    private int             target;

    private void OnEnable()
    {
        GameManager.instance.eventSystem.SetSelectedGameObject(contents.transform.GetChild(0).transform.gameObject);
        combinationButton.SetActive(false);
        MainCanvas.instance.PlaySoundOneShot(MainCanvas.instance.shop.Path);
    }
    private void Awake()
    {
        contentsRectTransform = contents.GetComponent<RectTransform>();
        for (int i = 0; i < DataManager.instance.useItemRecipeList.Count; i++)
        {
            CombiContentsUI temp = Instantiate(contentsPrefab, contents.transform).GetComponent<CombiContentsUI>();
            temp.image.sprite = DataManager.instance.useItemDic[DataManager.instance.useItemRecipeList[i].result].sprite;
            temp.textMeshProUGUI.text = DataManager.instance.useItemRecipeList[i].result;
            int tempint = i;
            temp.gameObject.GetComponent<Button>().onClick.AddListener(() => { CombiRecipeView(tempint); });
            contentsRectTransform.sizeDelta = new Vector2(contentsRectTransform.sizeDelta.x, contentsRectTransform.sizeDelta.y + 50);
        }
    }

    public void CombiRecipeView(int num)
    {
        target = num;   
        UseItem      useitem       = DataManager.instance.useItemDic[DataManager.instance.useItemRecipeList[num].result];
        MaterialItem materialItemA = DataManager.instance.materialsDic[DataManager.instance.useItemRecipeList[num].materialA];
        MaterialItem materialItemB = DataManager.instance.materialsDic[DataManager.instance.useItemRecipeList[num].materialB];
        int          resultNum     = WarehouseManager.instance.FindItem(useitem);
        int          materialAnum  = WarehouseManager.instance.FindItem(materialItemA);
        int          materialBnum  = WarehouseManager.instance.FindItem(materialItemB);

        resultImage.sprite         = useitem.sprite;
        materialAImage.sprite      = materialItemA.sprite;
        materialBImage.sprite      = materialItemB.sprite;

        resultContents.text        = useitem.contents;
        materialAName.text         = materialItemA.itemName + "1 (" + materialAnum + ")";
        materialBName.text         = materialItemB.itemName + "1 (" + materialBnum + ")";

        if(materialAnum>0 && materialBnum>0)
        {
            combinationButton.SetActive(true);
            GameManager.instance.eventSystem.SetSelectedGameObject(combinationButton);
        }
        else
        {
            combinationButton.SetActive(false);
            if(GameManager.instance.eventSystem.currentSelectedGameObject == combinationButton)
                GameManager.instance.eventSystem.SetSelectedGameObject(contents.transform.GetChild(0).transform.gameObject);
        }
        MainCanvas.instance.PlaySoundOneShot(MainCanvas.instance.buttonSound.Path);
    }

    public void Combination()
    {
        MainCanvas.instance.PlaySoundOneShot(MainCanvas.instance.mixSuccess.Path);
        WarehouseManager.instance.itemDelegate(DataManager.instance.useItemDic[DataManager.instance.useItemRecipeList[target].result]);
        WarehouseManager.instance.MinusItem(DataManager.instance.materialsDic[DataManager.instance.useItemRecipeList[target].materialA],1);
        WarehouseManager.instance.MinusItem(DataManager.instance.materialsDic[DataManager.instance.useItemRecipeList[target].materialB],1);
        CombiRecipeView(target);
    }
}
