using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : Singleton<GameManager>
{
    private int         setWidth    = 1920;
    private int         deviceWidth = Screen.width;
    public  float       ratio;
    public  EventSystem eventSystem;

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
