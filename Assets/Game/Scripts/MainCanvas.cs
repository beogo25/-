using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCanvas : MonoBehaviour
{
    public GameObject[] allUI;
    public GameObject blur;
    public GameObject menuUI;
    [HideInInspector]
    public Player player;
    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(blur.activeInHierarchy)
            {
                Exit();
            }
            else
            {
                menuUI.SetActive(true);
                blur.SetActive(true);
                player.talkState = true;
            }
        }
    }
    public void Exit()
    {
        for(int i = 0; i < allUI.Length; i++)
        {
            allUI[i].gameObject.SetActive(false);
        }
        blur.SetActive(false);
        player.talkState = false;
    }
}
