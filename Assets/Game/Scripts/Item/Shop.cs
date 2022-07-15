using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public GameObject contents;
    public GameObject contentsPrifab;
    private RectTransform contentsRectTransform;

    public Image selectImage;
    public TextMeshProUGUI selectContents;
    public TextMeshProUGUI selectName;
    public TextMeshProUGUI needGold;
    public TextMeshProUGUI playerGold;

    public GameObject purchaseButton;
    private string target;

    public string[] saleItem;
    private void OnEnable()
    {
        purchaseButton.SetActive(false);
    }
    private void Awake()
    {
        contentsRectTransform = contents.GetComponent<RectTransform>();
    }
    void Start()
    {
        for (int i = 0; i < saleItem.Length; i++)
        {
            CombiContentsUI temp = Instantiate(contentsPrifab, contents.transform).GetComponent<CombiContentsUI>();
            temp.image.sprite = DataManager.instance.useItemDic[saleItem[i]].sprite;
            temp.textMeshProUGUI.text = saleItem[i];
            int tempint = i;
            temp.gameObject.GetComponent<Button>().onClick.AddListener(() => { SaleItemView(saleItem[tempint]); });
            contentsRectTransform.sizeDelta = new Vector2(contentsRectTransform.sizeDelta.x, contentsRectTransform.sizeDelta.y + 50);
        }
        playerGold.text = "보유 금액 : " + PlayerStatus.gold.ToString();
    }
    public void SaleItemView(string input)
    {
        target = input;
        playerGold.text = "보유 금액 : " + PlayerStatus.gold.ToString();
        UseItem tempUseItem = DataManager.instance.useItemDic[input];
        selectImage.sprite = tempUseItem.sprite;
        selectContents.text = tempUseItem.contents;
        selectName.text = input;
        needGold.text = tempUseItem.value.ToString();
        if (PlayerStatus.gold >= tempUseItem.value)
            purchaseButton.SetActive(true);
        else
            purchaseButton.SetActive(false);
    }
    public  void Purchase()
    {
        if(!InventoryManager.instance.AddItem(DataManager.instance.useItemDic[target]))
        {
            WarehouseManager.instance.AddItem(DataManager.instance.useItemDic[target]); 
        }
        PlayerStatus.gold -= DataManager.instance.useItemDic[target].value;
        SaleItemView(target);
    }
}
