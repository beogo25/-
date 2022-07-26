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

    private void Update()
    {
        JoyPadCheck();
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

    private void JoyPadCheck()
    {
        //조이패드인지 키보드인지 확인하는 식
        string[] names = Input.GetJoystickNames();
        for (int i = 0; i < names.Length; i++)
        {
            if (names[i].Length == 33)
                isJoyPadOn = true;
            else
                isJoyPadOn = false;
        }
    }
}
