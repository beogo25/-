using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class TempSliderManager : MonoBehaviour
{
    [SerializeField]
    private Text field =null;

    [SerializeField]
    private Slider slider = null;

    [SerializeField]
    private string busPath = "";

    private FMOD.Studio.Bus bus;

    private void Start()
    {
        if (busPath != "")
        {
            bus = RuntimeManager.GetBus(busPath);
        }
        bus.getVolume(out float volume);
        slider.value = volume * slider.maxValue;
        UpdateSlider();
    }

    public void UpdateSlider()
    {
        if(field != null && slider != null)
        {
            field.text = slider.value.ToString();

            bus.setVolume(slider.value/slider.maxValue);
        }
    }
}
