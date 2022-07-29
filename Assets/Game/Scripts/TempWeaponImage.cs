using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TempWeaponImage : MonoBehaviour, IDragHandler, IScrollHandler
{
    public GameObject tempWeapon;
    public GameObject sideCamera;
    float horizontal;
    float vertical;
    float wheel;
    private void OnEnable()
    {
        sideCamera.transform.localPosition = new Vector3(sideCamera.transform.localPosition.x, sideCamera.transform.localPosition.y,-5);
        tempWeapon.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    public void OnDrag(PointerEventData eventData)
    {
        horizontal = Input.GetAxis("Mouse X");
        vertical = Input.GetAxis("Mouse Y");
        tempWeapon.transform.Rotate(new Vector3(vertical*2, -horizontal*2, 0), Space.World);
    }


    public void OnScroll(PointerEventData eventData)
    {
        wheel = Input.GetAxis("Mouse ScrollWheel");
        if(sideCamera.transform.localPosition.z<-3 && wheel > 0)
            sideCamera.transform.Translate(Vector3.forward * wheel * 2);
        if (sideCamera.transform.localPosition.z > -7 && wheel < 0)
            sideCamera.transform.Translate(Vector3.forward * wheel * 2);
    }
}
