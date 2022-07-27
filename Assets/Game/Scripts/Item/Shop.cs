using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public  GameObject      contents;
    public  GameObject      contentsPrefab;
    private RectTransform   contentsRectTransform;

    public  Image           selectImage;
    public  TextMeshProUGUI selectContents;
    public  TextMeshProUGUI selectName;
    public  TextMeshProUGUI needGold;
    public  TextMeshProUGUI playerGold;
            
    public  GameObject      purchaseButton;
    private string          target;

    public  GameObject      blur;
    private Player          player;
    public  string[]        saleItem;
    private void OnEnable()
    {
        if (GameManager.isJoyPadOn)
            GameManager.instance.eventSystem.SetSelectedGameObject(contents.transform.GetChild(0).transform.gameObject);
        purchaseButton.SetActive(false);
        MainCanvas.instance.PlaySoundOneShot(MainCanvas.instance.shop.Path);
    }
    private void Awake()
    {
        contentsRectTransform = contents.GetComponent<RectTransform>();
        player = FindObjectOfType<Player>();
        for (int i = 0; i < saleItem.Length; i++)
        {
            CombiContentsUI temp            = Instantiate(contentsPrefab, contents.transform).GetComponent<CombiContentsUI>();
            temp.image.sprite               = DataManager.instance.useItemDic[saleItem[i]].sprite;
            temp.textMeshProUGUI.text       = saleItem[i];
            int tempint                     = i;
            contentsRectTransform.sizeDelta = new Vector2(contentsRectTransform.sizeDelta.x, contentsRectTransform.sizeDelta.y + 50);

            temp.gameObject.GetComponent<Button>().onClick.AddListener(() => { SaleItemView(saleItem[tempint]); });
        }
        playerGold.text = "보유 금액 : " + PlayerStatus.gold.ToString();
    }
    public void SaleItemView(string input)
    {
        target              = input;
        playerGold.text     = "보유 금액 : " + PlayerStatus.gold.ToString();
        UseItem tempUseItem = DataManager.instance.useItemDic[input];
        selectImage.sprite  = tempUseItem.sprite;
        selectContents.text = tempUseItem.contents;
        selectName.text     = input;
        needGold.text       = tempUseItem.value.ToString() + " 골드";
        if (PlayerStatus.gold >= tempUseItem.value)
        {
            purchaseButton.SetActive(true);
            if (GameManager.isJoyPadOn)
                GameManager.instance.eventSystem.SetSelectedGameObject(purchaseButton);
        }
        else
        {
            purchaseButton.SetActive(false);
            if (GameManager.isJoyPadOn)
            {
                if (GameManager.instance.eventSystem.currentSelectedGameObject == purchaseButton)
                    GameManager.instance.eventSystem.SetSelectedGameObject(contents.transform.GetChild(0).transform.gameObject);
            }

        }
        MainCanvas.instance.PlaySoundOneShot(MainCanvas.instance.buttonSound.Path);
    }
    public  void Purchase()
    {
        if(!InventoryManager.instance.AddItem(DataManager.instance.useItemDic[target]))
        {
            WarehouseManager.instance.AddItem(DataManager.instance.useItemDic[target]); 
        }
        PlayerStatus.gold -= DataManager.instance.useItemDic[target].value;
        MainCanvas.instance.PlaySoundOneShot(MainCanvas.instance.bigButton.Path);
        SaleItemView(target);
    }
}
