using UnityEngine;

public class LockOn : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform targetTransform;
    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }
    void Update()
    {
        transform.LookAt(cameraTransform.position);
        transform.eulerAngles = cameraTransform.eulerAngles;
        transform.position    = targetTransform.position;
    }
}
