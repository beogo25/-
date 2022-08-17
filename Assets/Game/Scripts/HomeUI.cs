using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeUI : MonoBehaviour
{
    [SerializeField] 
    private GameObject menu;
    [SerializeField] 
    private GameObject exitUI;
    [SerializeField] 
    private GameObject OptionUI;
    private GameObject newButton;
    private GameObject exitYesButton;
    private bool       isMenuOn;

    private void Start()
    {
        newButton     =   menu.transform.GetChild(1).gameObject;
        exitYesButton = exitUI.transform.GetChild(1).gameObject;
    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            if(OptionUI.activeInHierarchy)
            {
                OptionUI.SetActive(false);
                menu.SetActive(true);
                if(GameManager.isJoyPadOn)
                    GameManager.instance.SetPosition(newButton);
                isMenuOn = false;
            }
            else if(exitUI.activeInHierarchy)
            {
                exitUI.SetActive(false);
                menu.SetActive(true);
                if (GameManager.isJoyPadOn)
                    GameManager.instance.SetPosition(newButton);
                isMenuOn = false;
            }
            else
            {
                exitUI.SetActive(true);
                menu.SetActive(false);
                if (GameManager.isJoyPadOn)
                    GameManager.instance.SetPosition(exitYesButton);
            }
        }

        if(!isMenuOn && GameManager.isJoyPadOn)
        {
            GameManager.instance.SetPosition(newButton);
            isMenuOn = true;
        }
    }
}
