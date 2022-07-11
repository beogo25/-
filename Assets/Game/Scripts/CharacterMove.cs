using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float movementSpeed;
    public float jumpPower;

    private Rigidbody characterRigidbody;
    private Transform characterBody;

    private void Start()
    {
        characterRigidbody = transform.GetChild(0).GetComponent<Rigidbody>();
        characterBody = transform.GetChild(0).transform;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 moveDir = new Vector3().GetDirectionByCamera(Camera.main);
        if (Player.isMoveAble)
        {
            transform.position += moveDir * Time.deltaTime * movementSpeed;

            if (moveDir.x == 0 && moveDir.z == 0)
                characterBody.rotation = characterBody.rotation;
            else
                characterBody.forward = moveDir;
        }
    }
    public void Jump()          
    {
        characterRigidbody.velocity = Vector3.zero;
        characterRigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
}
