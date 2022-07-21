using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class WarehouseUI : MonoBehaviour
{
    public ItemInformationSystem itemInformation;
    public int selectSlot = 0;
    public WarehouseSlot[] slots;
    public GameObject[] exitTarget;
    public EventSystem eventSystem;

    private Player player;
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
        player = FindObjectOfType<Player>();
        for (int i = 0; i < slots.Length; i++)
            slots[i].num = i;
    }
    private void OnValidate()
    {
        slots = transform.GetComponentsInChildren<WarehouseSlot>();
    }
    private void OnEnable()
    {
        Refresh();
    }
    public virtual void ItemInformationChange(int num)
    {
        itemInformation.WareHouseBool = true;
        itemInformation.targetNum = num;
    }
    public abstract void Refresh();
}
