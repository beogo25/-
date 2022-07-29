using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WarehouseUI : MonoBehaviour
{
    public ItemInformationSystem itemInformation;
    public int                   selectSlot = 0;
    public WarehouseSlot[]       slots;
    public GameObject[]          exitTarget;

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
        slots = transform.GetComponentsInChildren<WarehouseSlot>();
    }
    public virtual void OnEnable()
    {
        Refresh();
        MainCanvas.instance.PlaySoundOneShot(MainCanvas.instance.itemBox.Path);
    }
    public virtual void ItemInformationChange(int num)
    {
        itemInformation.ButtonSet(true);
        itemInformation.targetNum = num;
    }
    public abstract void Refresh();
}
