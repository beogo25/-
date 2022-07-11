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
        target = new string[3] { "»ç½¿»Ô", "»ç½¿°¡Á×", "»ç½¿»À" };
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
        textname.text    = ItemManager.materialsDic[input].itemName;
        textvalue.text   = ItemManager.materialsDic[input].value.ToString();
        textcontent.text = ItemManager.materialsDic[input].stack.ToString();
        image.sprite     = ItemManager.materialsDic[input].sprite;
    }

    public void InputItem()
    {
        WarehouseManager.Instance.AddItem(ItemManager.materialsDic[target[num]]);
    }
}
