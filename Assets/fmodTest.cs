using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class fmodTest : MonoBehaviour
{
    //���� �̺�Ʈ
    [SerializeField]
    public EventReference soundEvent;

    //�Ű����� 
    [SerializeField]
    [ParamRef]
    public string power;
    public void PlayOneshot()
    {
        if(!soundEvent.IsNull)
        {
            RuntimeManager.PlayOneShot(soundEvent.Path);
        }
    }
    //�Ű�����
    public void ParamPlay()
    {
        
        FMOD.Studio.EventInstance eventInstance = RuntimeManager.CreateInstance(soundEvent.Path);
        //3D
        //eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
        //�Ű����� ��������
        //eventInstance.setParameterByName("�Ű����� �̸�", ��ǲ��)
        //�� : �̷��� �־ ���� ������ 5�����߿��� �������� ������ ������ �����ҵ�
        //eventInstance.setParameterByName("AttackSlash", Random.Range(0, 4))
        //�Ű����� �ٲٱ�
        //
        eventInstance.start();
        eventInstance.release();
    }
}
