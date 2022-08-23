using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class MainCanvas : Singleton<MainCanvas>
{
    public GameObject[] allUI;
    public GameObject blur;
    public GameObject menuUI;
    [HideInInspector]
    public Player player;
    public EventReference buttonSound;
    public EventReference getQuest;
    public EventReference itemMove;
    public EventReference cancelMenu;
    public EventReference bigButton;
    public EventReference itemBox;
    public EventReference nextQuest;
    public EventReference shop;
    public EventReference questOpen;
    public EventReference mixSuccess;
    public EventReference equipmentCombi;
    public EventReference bubble;
    public EventReference questClear;

    public void PlaySoundOneShot(string input)
    {
        RuntimeManager.PlayOneShot(input);
    }
    public override void Awake()
    {
        base.Awake();
        player = FindObjectOfType<Player>();

        // 커서 화면에 안보이도록
        Cursor.visible = false;
        // 커서 화면에 고정
        Cursor.lockState = CursorLockMode.Locked;
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
        else if (player.IsMapOpen == true)
        {
            if (Input.GetButtonDown("Start") || Input.GetButtonDown("Cancel"))
                player.IsMapOpen = false;
        }
        else
        {
            if(Input.GetButtonDown("Start"))
            {
                if (GameManager.isJoyPadOn)
                    GameManager.instance.eventSystem.SetSelectedGameObject(menuUI.transform.GetChild(0).gameObject);
                menuUI.SetActive(true);
                blur.SetActive(true);
                player.TalkState = true;
            }
        }

    }
    public void Exit()
    {
        for(int i = 0; i < allUI.Length - 1; i++)
        {
            allUI[i].gameObject.SetActive(false);
        }
        blur.SetActive(false);
        player.TalkState = false;

        StartCoroutine(Stop());
        RuntimeManager.PlayOneShot(cancelMenu.Path);
        if(TalkManager.instance.gameObject.activeInHierarchy)
        {
            for (int i = 0; i < TalkManager.instance.UIButton.Length; i++)
                TalkManager.instance.UIButton[i].SetActive(false);
            TalkManager.instance.talkUI.SetActive(false);
        }
    }

    private IEnumerator Stop()
    {
        Player.isMoveAble = false;
        yield return new WaitForSeconds(0.3f);
        Player.isMoveAble = true;
    }
}
