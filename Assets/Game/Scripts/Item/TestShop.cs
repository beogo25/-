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
        target = new string[3] { "Ã¶°Ë","°­Ã¶°Ë","Ã¶°©¿Ê" };
    }
    void Start()
    {
        
    }

    void Update()
    {

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
        textname.text    = ItemListManager.instance.equipmentDic[input].itemName;
        textvalue.text   = ItemListManager.instance.equipmentDic[input].value.ToString();
        textcontent.text = ItemListManager.instance.equipmentDic[input].contents;
        image.sprite     = ItemListManager.instance.equipmentDic[input].sprite;
    }

    public void InputItem()
    {
        WarehouseManager.instance.AddItem(ItemListManager.instance.equipmentDic[target[num]]);
    }
}
