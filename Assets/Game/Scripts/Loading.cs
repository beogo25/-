using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    private Slider slider;
    public string sceneName;
    private void Awake()
    {
        slider = GetComponent<Slider>();
        
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Z))
            StartCoroutine(LoadAsynSceneCoroutine());
    }

    void Start()
    {
        
    }

    IEnumerator LoadAsynSceneCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            slider.value = operation.progress;
            if(operation.progress>=0.9f)
            {
                break;
            }
        }
        slider.value = 1;
        yield return new WaitForSeconds(3);
        operation.allowSceneActivation = true;
    }
}
