using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        PlayerStatus player = other.GetComponent<PlayerStatus>(); 

        if (player != null)  // 콜리전 충돌일어난게 플레이어라면
        {
            Debug.Log(other.name + "콜리전충돌");
            player.PlayerHit(0, 0,Vector3.zero, AttackType.BURN);
        }
    }
}
