using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialWarehouse : Warehouse
{
    public ItemInformation itemInformation;
    public int             selectSlot = 0;
    public Slot[]          slots;
    public int SelectSlot
    {
        get { return selectSlot; }
        set
        {
            selectSlot = value;
            //ui표시해주기
        }
    }
    private void Awake()
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].num = i;
    }
    private void OnValidate()
    {
        slots = transform.GetComponentsInChildren<Slot>();
    }
    private void OnEnable()
    {
        int num = slots.Length;
        if (num > WarehouseManager.Instance.materialItemList.Count)
            num = WarehouseManager.Instance.materialItemList.Count;
        for (int i = 0; i < num; i++)
        {
            slots[i].image.sprite = WarehouseManager.Instance.materialItemList[i].sprite;
            slots[i].stack.text = WarehouseManager.Instance.materialItemList[i].stack.ToString();
            slots[i].gameObject.SetActive(true);
        }
        for (int i = num; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
    }

    public override void ItemInformationChange(int num)
    {
        Debug.Log(num);
        itemInformation.image.sprite  = WarehouseManager.Instance.materialItemList[num].sprite;
        itemInformation.itemName.text = WarehouseManager.Instance.materialItemList[num].itemName;
        itemInformation.stack.text    = WarehouseManager.Instance.materialItemList[num].stack.ToString();
        itemInformation.contents.text = WarehouseManager.Instance.materialItemList[num].contents;
    }
}
