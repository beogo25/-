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
    public WarehouseUI     warehouse;
    public Button          button;
    private void OnValidate()
    {
        image     = transform.GetChild(0).GetComponent<Image>();
        stack     = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        button    = transform.GetChild(2).GetComponent<Button>();
        warehouse = transform.parent.GetComponent<WarehouseUI>();
    }
    public void OnClickEvent()
    {
        if(image.sprite != null)
        {
            warehouse.ItemInformationChange(num);
            MainCanvas.instance.PlaySoundOneShot(MainCanvas.instance.buttonSound.Path);
        }
    }
}
