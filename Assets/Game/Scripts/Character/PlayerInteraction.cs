using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
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

    [SerializeField]
    private EventReference  getItem;
                            
    [SerializeField]        
    private Sprite[]        keySprite;
    private Image           keyImage;
    private bool            isPlayed = false;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        keyImage = ItemUI.transform.GetChild(0).GetChild(1).GetComponent<Image>();
        ItemUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        InteractionFunc();
    }

    private void InteractionFunc()
    {
        Collider[] iconDisplay = Physics.OverlapSphere(transform.position, 3f, (1 << LayerMask.NameToLayer("Collective") | 1 << LayerMask.NameToLayer("Npc")));
        if (iconDisplay.Length > 0)
        {
            Transform nearestTarget = iconDisplay[0].transform;
            float nearestDis = 3;

            for (int i = 0; i < iconDisplay.Length; i++)
            {
                float tempDis = Vector3.Distance(iconDisplay[i].transform.position, transform.position);
                if (tempDis <= nearestDis)
                {
                    nearestDis = tempDis;
                    nearestTarget = iconDisplay[i].transform;
                }
            }

            if (!player.TalkState)
            {
                if (!ItemUI.activeInHierarchy)
                    ItemUI.SetActive(true);

                ItemUI.transform.LookAt(Camera.main.transform.position);

                //NPC
                if (nearestTarget.GetComponent<Npc>() != null)
                {
                    if (isPlayed == false && ItemUI.activeInHierarchy)
                    {
                        isPlayed = true;
                        MainCanvas.instance.PlaySoundOneShot(MainCanvas.instance.bubble.Path);
                    }

                    Player.isRollAble = false;
                    if (text.text != nearestTarget.transform.GetComponent<Npc>().npcName)
                    {
                        text.text = nearestTarget.transform.GetComponent<Npc>().npcName;
                        image.sprite = nearestTarget.transform.GetComponent<Npc>().minimapIcon.GetComponent<SpriteRenderer>().sprite;
                        if (GameManager.isJoyPadOn)
                            keyImage.sprite = keySprite[2];
                        else
                            keyImage.sprite = keySprite[0];
                    }
                    if (Input.GetButtonDown("InteractionNpc") && player.isGround)
                    {
                        if (nearestTarget.transform.GetComponent<IInteraction>() != null)
                            nearestTarget.transform.GetComponent<IInteraction>().Interaction();
                    }

                }

                //채집물
                else if (nearestTarget.GetComponent<InteractionObject>() != null)
                {

                    //플레이어 채집 가능 상태 ON
                    player.isCollectable = true;

                    //이름과 그림을 알맞게 맞춰줌
                    if (text.text != nearestTarget.GetComponent<InteractionObject>().objectName)
                    {
                        text.text = nearestTarget.GetComponent<InteractionObject>().objectName;
                        image.sprite = nearestTarget.GetComponent<InteractionObject>().imageSprite;
                    }

                    //채집 가능할때 
                    if (nearestTarget.GetComponent<InteractionObject>().isCollectable)
                    {
                        if (isPlayed == false && ItemUI.activeInHierarchy)
                        {
                            isPlayed = true;
                            MainCanvas.instance.PlaySoundOneShot(MainCanvas.instance.bubble.Path);
                        }


                        if (GameManager.isJoyPadOn)
                            keyImage.sprite = keySprite[1];
                        else
                            keyImage.sprite = keySprite[0];
                        if (Input.GetButtonDown("InteractionObject") && player.isGround)
                        {
                            nearestTarget.GetComponent<InteractionObject>().Interaction();
                            RuntimeManager.PlayOneShot(getItem.Path);
                        }

                    }
                    else
                    {
                        if (ItemUI.activeInHierarchy)
                            ItemUI.SetActive(false);
                    }

                }


            }
        }
        else
        {
            if (ItemUI.activeInHierarchy)
                ItemUI.SetActive(false);
            isPlayed = false;
            Player.isRollAble = true;
        }
    }
}
