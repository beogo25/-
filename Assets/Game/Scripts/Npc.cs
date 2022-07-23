using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour, IInteraction
{
    public string npcName;
    [TextArea]
    public string[] talkText;
    public Sprite standing;
    public UIType[] uiTypes;
    public Animator[] animator;
    Player player;
    private GameObject minimapIcon;
    private Vector3 minimapRotate;
    private CameraMove cameraMove;

    private void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.layer == LayerMask.NameToLayer("miniMap"))
                minimapIcon = transform.GetChild(i).gameObject;
        }
        cameraMove = FindObjectOfType<CameraMove>();
        player = FindObjectOfType<Player>();
        StartCoroutine(IdleMotion());
        minimapRotate = minimapIcon.transform.eulerAngles;
    }
    public void Interaction()
    {
        TalkManager.instance.TalkStart(npcName, talkText, standing, uiTypes);
        StartCoroutine(cameraMove.CameraFocus(this.gameObject));
        
        float angle = transform.rotation.eulerAngles.y;
        transform.LookAt(player.gameObject.transform.position);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        minimapIcon.transform.eulerAngles = minimapRotate;

        for(int i = 0; i < animator.Length; i++)
            animator[i].SetTrigger("Interaction");
    }
    IEnumerator IdleMotion()
    {
        while(true)
        {
            for (int i = 0; i < animator.Length; i++)
                animator[i].SetTrigger("Interaction");
            yield return new WaitForSecondsRealtime(Random.Range(50,60));
        }
    }
}
