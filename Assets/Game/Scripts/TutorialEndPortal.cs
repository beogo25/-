using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEndPortal : MonoBehaviour
{
    public string SceneName;


    private void OnTriggerEnter(Collider other)
    {
            Debug.Log("�� �̵�");
            SceneManager.LoadScene(SceneName);
     
    }
}
