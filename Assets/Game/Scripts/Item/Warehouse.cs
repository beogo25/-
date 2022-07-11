using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Warehouse : MonoBehaviour
{
    public ItemInformation itemInformation;
    public int selectSlot = 0;
    public Slot[] slots;
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
    public abstract void ItemInformationChange(int num);
}
