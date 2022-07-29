using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class fmodTest : MonoBehaviour
{
    //사운드 이벤트
    [SerializeField]
    public EventReference soundEvent;

    //매개변수 
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
    //매개변수
    public void ParamPlay()
    {
        
        FMOD.Studio.EventInstance eventInstance = RuntimeManager.CreateInstance(soundEvent.Path);
        //3D
        //eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.transform));
        //매개변수 가져오기
        //eventInstance.setParameterByName("매개변수 이름", 인풋값)
        //예 : 이렇게 넣어서 어택 슬래쉬 5가지중에서 랜덤으로 음성이 나오게 가능할듯
        //eventInstance.setParameterByName("AttackSlash", Random.Range(0, 4))
        //매개변수 바꾸기
        //
        eventInstance.start();
        eventInstance.release();
    }
}
