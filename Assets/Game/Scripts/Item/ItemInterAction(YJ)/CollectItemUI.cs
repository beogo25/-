using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CollectItemUI : MonoBehaviour
{
    public Image  getItemSprite;
    public TextMeshProUGUI getItemName;

    private void Start()
    {
        WarehouseManager.instance.itemDelegate += SetItemDisplay;
    }
    void SetItemDisplay(Item item)
    {
        getItemName.text = item.itemName;
        getItemSprite.sprite = item.sprite;
    }
}
