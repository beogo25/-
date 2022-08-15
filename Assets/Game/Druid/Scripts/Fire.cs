using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        PlayerStatus player = other.GetComponent<PlayerStatus>(); 

        if (player != null)  // �ݸ��� �浹�Ͼ�� �÷��̾���
        {
            Debug.Log(other.name + "�ݸ����浹");
            player.PlayerHit(0, 0,Vector3.zero, AttackType.BURN);
        }
    }
}
