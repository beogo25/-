using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitUI : MonoBehaviour
{
    private void OnEnable()
    {
        GameManager.instance.eventSystem.SetSelectedGameObject(transform.GetChild(1).gameObject);

    }

}
