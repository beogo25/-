using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour, IInteraction
{
    public string npcName;
    [Multiline]
    public string[] talkText;
    public Sprite standing;
    public UIType[] uiTypes;
    private void Start()
    {
        Interaction();
    }
    public void Interaction()
    {
        TalkManager.instance.TalkStart(npcName, talkText, standing,uiTypes);  
    }
}
