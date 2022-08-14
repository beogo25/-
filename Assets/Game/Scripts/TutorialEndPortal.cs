using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEndPortal : MonoBehaviour
{
    

    private void OnTriggerEnter(Collider other)
    {
        GameManager.instance.LoadorNew(0);
    }
}
