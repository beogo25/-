using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private int         setWidth    = 1920;
    private int         deviceWidth = Screen.width;
    public  float       ratio;
    public  EventSystem eventSystem;
    public  bool        load        = false; //새로시작,로드에서 변경
    public  static bool isJoyPadOn;
    public  static bool isFullScreen;
    public GameObject   checkBox;

    public override void Awake()
    {
        base.Awake();
        //해상도 60프레임 고정
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        JoyPadCheck();
        ratio = (float)deviceWidth / (float)setWidth;
    }
    
    public void LoadorNew(int name)
    {
        string sceneName = "";
        switch(name)
        {
            case (int)SceneName.LOAD:
                sceneName = "GameScene";
                load = true;
                break;
            case (int)SceneName.NEW:
                sceneName = "Tutorial";
                break;
            case (int)SceneName.MAIN:
                sceneName = "MainScene";
                break;
            default:
                break;
        }
        if(sceneName != "")
        {
            SceneManager.LoadScene("Loading");
            StartCoroutine(WaitSceneChange(sceneName));
        }
    }
    IEnumerator WaitSceneChange(string sceneName)
    {
        while (true)
        {
            if (SceneManager.GetActiveScene().name == "Loading")
                break;

            yield return new WaitForSecondsRealtime(0.1f);
        }
        yield return new WaitForSecondsRealtime(1);
        FindObjectOfType<Loading>().SceneName = sceneName;
    }
    public void SetPosition(GameObject selected)
    {
        if(selected != null && isJoyPadOn)
            eventSystem.SetSelectedGameObject(selected);
    }
    public void ApplicationExit()
    {
        Application.Quit();
    }

    public void SetFullScreen()
    {
        if (isFullScreen == true)
        {
            Screen.SetResolution(1920, 1080, true);
            checkBox.SetActive(true);
            isFullScreen = false;
        }
        else
        {
            Screen.SetResolution(1920, 1080, false);
            checkBox.SetActive(false);
            isFullScreen = true;
        }
    }

    private void JoyPadCheck()
    {
        //조이패드인지 키보드인지 확인하는 식
        string[] names = Input.GetJoystickNames();
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Length > 0)
                isJoyPadOn = true;
            else
                isJoyPadOn = false;
        }
    }
}
