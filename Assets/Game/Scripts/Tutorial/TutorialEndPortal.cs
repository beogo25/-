using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEndPortal : MonoBehaviour
{
    public GameObject[] destroyObjects;

    private void OnTriggerEnter(Collider other)
    {
        DestroyObjects();
        GameManager.instance.LoadorNew(0);
    }

    public void DestroyObjects()
    {
        for (int i = 0; i < destroyObjects.Length; i++)
            Destroy(destroyObjects[i]);
    }
}
