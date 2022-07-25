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

        if (blur.activeInHierarchy)
        {
            if (Input.GetButtonDown("Start") || Input.GetButtonDown("Cancel"))
            {
                Exit();
            }
        }
        else
        {
            if(Input.GetButtonDown("Start"))
            {
                GameManager.instance.eventSystem.SetSelectedGameObject(menuUI.transform.GetChild(0).gameObject);
                menuUI.SetActive(true);
                blur.SetActive(true);
                player.TalkState = true;
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
        //allUI[14].SetActive(true);
        player.TalkState = false;
        allUI[14].SetActive(true);
        StartCoroutine(Stop());
    }

    private IEnumerator Stop()
    {
        Player.isMoveAble = false;
        yield return new WaitForSeconds(0.3f);
        Player.isMoveAble = true;
    }
}
