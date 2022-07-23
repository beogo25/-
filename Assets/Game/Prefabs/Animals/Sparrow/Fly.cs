using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    private int randomAngleY;
    private int randomAngleX;
    private Vector3 firstPosition;
    private void Awake()
    {
        firstPosition = transform.position;
    }

    private void OnEnable()
    {
        randomAngleY = Random.Range(-15, 15);
        randomAngleX = Random.Range(-10, 10);
        transform.eulerAngles = new Vector3(randomAngleX, randomAngleY, 0);
    }

    // Update is called once per frame
    void Update()
    {
            transform.Translate(Vector3.forward*0.4f, Space.Self);
    }
}
