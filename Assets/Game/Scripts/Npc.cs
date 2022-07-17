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

    private CameraMove cameraMove;

    private void Awake()
    {
        cameraMove = FindObjectOfType<CameraMove>();
        player = FindObjectOfType<Player>();   
        StartCoroutine(IdleMotion());   
    }
    public void Interaction()
    {
        TalkManager.instance.TalkStart(npcName, talkText, standing,uiTypes);
        StartCoroutine(cameraMove.CameraFocus(this.gameObject));
        transform.LookAt(player.gameObject.transform.position);
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
