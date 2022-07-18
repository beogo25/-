using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WarehouseUI : MonoBehaviour
{
    public ItemInformationSystem itemInformation;
    public int selectSlot = 0;
    public WarehouseSlot[] slots;
    public GameObject[] exitTarget;
    
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
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ExiteButton();
    }
    public virtual void ItemInformationChange(int num)
    {
        itemInformation.WareHouseBool = true;
        itemInformation.targetNum = num;
    }

    public void ExiteButton()
    {
        for(int i = 0; i < exitTarget.Length; i++)
            exitTarget[i].SetActive(false);
        player.talkState = false;
        itemInformation.gameObject.SetActive(false);
    }
    public abstract void Refresh();
}
