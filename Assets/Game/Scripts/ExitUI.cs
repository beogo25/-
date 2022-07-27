using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitUI : MonoBehaviour
{
    private void OnEnable()
    {
        if(GameManager.isJoyPadOn)
            GameManager.instance.eventSystem.SetSelectedGameObject(transform.GetChild(1).gameObject);

    }

}
