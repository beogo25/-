using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WarehouseSlot : MonoBehaviour
{
    public Image           image;
    public TextMeshProUGUI stack;
    public int             num;
    public WarehouseUI       warehouse;
    private void OnValidate()
    {
        image     = GetComponent<Image>();
        stack     = transform.GetChild(0).GetComponent<TextMeshProUGUI>(); 
        warehouse = transform.parent.GetComponent<WarehouseUI>();
    }
    public void OnClickEvent()
    {
        if(image.sprite != null)
            warehouse.ItemInformationChange(num);
    }
}
