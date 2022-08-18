using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcUI : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform.position);
    }
}
