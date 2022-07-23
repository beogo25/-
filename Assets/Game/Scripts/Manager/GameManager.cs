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
    public  bool        load=false; //���ν���,�ε忡�� ����

    public override void Awake()
    {
        base.Awake();
        //�ػ� 60������ ����
        Application.targetFrameRate = 60;
        //�ػ� 1920x1080���� ����
        //Screen.SetResolution(3840, 2160, true);
        Screen.SetResolution(1920, 1080, true);
        //�ػ󵵸� �ٲܶ��� UI�� ������ �������� ������ �� �ְ� ���ִ� ������ �޴´�.
        ratio = (float)deviceWidth / (float)setWidth;
    }
    public void LoadorNew(bool load)
    {
        this.load = load;
        SceneManager.LoadScene("Loading");
    }
    public void ApplicationExit()
    {
        Application.Quit();
    }
}
