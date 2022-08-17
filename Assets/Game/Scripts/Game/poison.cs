using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class poison : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Player>() != null)
            other.gameObject.GetComponent<Player>().status.PlayerHit(1, 0, Vector3.zero, AttackType.POISON);
    }

}
