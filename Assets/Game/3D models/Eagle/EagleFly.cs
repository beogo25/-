using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleFly : MonoBehaviour
{
    private Vector3 point;
    private float speed;


    private void Start()
    {
        StartCoroutine(GetRandomPosition());
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        transform.Rotate(point * 20 * Time.deltaTime);
    }

    IEnumerator GetRandomPosition()
    {
        while(true)
        {
            speed = Random.Range(30, 40);
            float randNumY = Random.Range(-1f, 1f);
            point = new Vector3(0, randNumY, 0);
            yield return new WaitForSeconds(20f);
        }
    }
}
