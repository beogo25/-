using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EagleFly : MonoBehaviour
{
    private Vector3 point;
    private Vector3 spawnPoint;
    private float speed;

    [SerializeField] Collider area;
    private void Start()
    {
        StartCoroutine(GetRandomPosition());
        spawnPoint = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);
        transform.Rotate(point * 20 * Time.deltaTime);
        PositionCheck();
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

    void PositionCheck()
    {
        if (transform.position.x < area.bounds.min.x || transform.position.x > area.bounds.max.x
            || transform.position.z < area.bounds.min.z || transform.position.z > area.bounds.max.z)
            transform.position = spawnPoint;
    }
}
