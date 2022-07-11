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
        target = new string[2] { "Ã¶°Ë","°­Ã¶°Ë" };
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
        textname.text    = ItemManager.instance.equipmentDic[input].itemName;
        textvalue.text   = ItemManager.instance.equipmentDic[input].value.ToString();
        textcontent.text = ItemManager.instance.equipmentDic[input].contents;
        image.sprite     = ItemManager.instance.equipmentDic[input].sprite;
    }

    public void InputItem()
    {
        //WarehouseManager.Instance.AddItem(ItemManager.materialsDic[target[num]]);
        //WarehouseManager.Instance.AddItem(ItemManager.UseItemDic[target[num]]);
        WarehouseManager.instance.AddItem(ItemManager.instance.equipmentDic[target[num]]);
    }
}
