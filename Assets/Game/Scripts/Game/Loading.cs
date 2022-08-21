using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public string sceneName;

    public string SceneName
    {
        get { return sceneName; }   
        set 
        { 
            sceneName = value;
            StartCoroutine(LoadAsynSceneCoroutine(sceneName));
        }  
    }
    IEnumerator LoadAsynSceneCoroutine(string sceneName)
    {
        yield return new WaitForSeconds(1);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        while (!operation.isDone)
        {
            if(operation.progress>=0.9f)
            {
                break;
            }
        }
        yield return new WaitForSeconds(2);
        operation.allowSceneActivation = true;
        
    }
}
