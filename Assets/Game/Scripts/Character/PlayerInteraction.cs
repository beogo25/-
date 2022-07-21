using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private GameObject      ItemUI;
    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private Image           image;

    private Player          player;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        InteractionFunc();
    }

    private void InteractionFunc()
    {
        Collider[] nearTarget = Physics.OverlapSphere(transform.position, 2f, 1 << LayerMask.NameToLayer("Collective"));
        if (nearTarget.Length > 0)
        {
            player.isCollectable = true;
            Transform nearestTarget = nearTarget[0].transform;
            float nearestDis = 2;

            for (int i = 0; i < nearTarget.Length; i++)
            {
                float tempDis = Vector3.Distance(nearTarget[i].transform.position, transform.position);
                if (tempDis <= nearestDis)
                {
                    nearestDis = tempDis;
                    nearestTarget = nearTarget[i].transform;
                }
            }
            if (nearestTarget.GetComponent<InteractionObject>().isCollectable)
                ItemUI.SetActive(true);
            else
                ItemUI.SetActive(false);

            ItemUI.transform.LookAt(Camera.main.transform.position);

            if (text.text != nearestTarget.GetComponent<InteractionObject>().objectName)
            {
                text.text = nearestTarget.GetComponent<InteractionObject>().objectName;
                image.sprite = nearestTarget.GetComponent<InteractionObject>().imageSprite;
            }

            if (Input.GetButtonDown("InteractionObject") && player.isGround)
            {
                if (nearestTarget.GetComponent<InteractionObject>() != null)
                    nearestTarget.GetComponent<InteractionObject>().Interaction();
            }
        }
        else
        {
            player.isCollectable = false;
            ItemUI.SetActive(false);
        }

        if (!player.talkState)
        {
            Collider[] npcTarget = Physics.OverlapSphere(transform.position, 2f, 1 << LayerMask.NameToLayer("Npc"));
            if (npcTarget.Length > 0)
            {
                player.isRollAble = false;
                if (Input.GetButtonDown("InteractionNpc") && player.isGround)
                {
                    if (npcTarget[0].transform.GetComponent<IInteraction>() != null)
                        npcTarget[0].transform.GetComponent<IInteraction>().Interaction();
                }
                else
                {
                    player.isRollAble = true;
                }
            }
        }
    }
}
