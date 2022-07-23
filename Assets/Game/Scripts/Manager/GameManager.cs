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
    public  bool        load=false; //새로시작,로드에서 변경

    public override void Awake()
    {
        base.Awake();
        //해상도 60프레임 고정
        Application.targetFrameRate = 60;
        //해상도 1920x1080으로 시작
        //Screen.SetResolution(3840, 2160, true);
        Screen.SetResolution(1920, 1080, true);
        //해상도를 바꿀때도 UI가 일정한 간격으로 움직일 수 있게 해주는 비율을 받는다.
        ratio = (float)deviceWidth / (float)setWidth;

        // 커서 화면에 안보이도록
        Cursor.visible = false;
        // 커서 화면에 고정
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void LoadorNew(bool load)
    {
        this.load = load;
        SceneManager.LoadScene("Loading");
    }
    public void SetPosition(GameObject selected)
    {
        if(selected != null)
            eventSystem.SetSelectedGameObject(selected);
    }
    public void ApplicationExit()
    {
        Application.Quit();
    }
}
