using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestShop : MonoBehaviour
{
    public Image     image;
    public Text      textname;
    public Text      textvalue;
    public Text      textcontent;
    private string[] target;
    private int      num = 0;
    private void Awake()
    {
        target = new string[3] { "Æ÷¼Ç","ºñ¾à","ÇØµ¶Á¦" };
    }
    private void Start()
    {
        WarehouseManager.instance.AddItem(JsonManager.instance.materialsDic["»ç½¿»Ô"]);
        WarehouseManager.instance.AddItem(JsonManager.instance.materialsDic["»ç½¿»Ô"]);
        WarehouseManager.instance.AddItem(JsonManager.instance.materialsDic["»ç½¿»Ô"]);
        WarehouseManager.instance.AddItem(JsonManager.instance.materialsDic["»ç½¿»Ô"]);
        WarehouseManager.instance.AddItem(JsonManager.instance.materialsDic["»ç½¿°¡Á×"]);
        WarehouseManager.instance.AddItem(JsonManager.instance.materialsDic["»ç½¿°¡Á×"]);
        WarehouseManager.instance.AddItem(JsonManager.instance.materialsDic["»ç½¿°¡Á×"]);
        WarehouseManager.instance.AddItem(JsonManager.instance.materialsDic["»ç½¿°¡Á×"]);
    }
    public void ButtonEvent(int input)
    {
        num += input;
        if (num < 0)
            num = target.Length - 1;
        if (num == target.Length)
            num = 0;
        UiChange(target[num]);
    }

    void UiChange(string input)
    {
        textname.text    = JsonManager.instance.useItemDic[input].itemName;
        textvalue.text   = JsonManager.instance.useItemDic[input].value.ToString();
        textcontent.text = JsonManager.instance.useItemDic[input].contents;
        image.sprite     = JsonManager.instance.useItemDic[input].sprite;
    }

    public void InputItem()
    {
        WarehouseManager.instance.AddItem(JsonManager.instance.useItemDic[target[num]]);
    }
}
