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
    public void Interaction()
    {
        TalkManager.instance.TalkStart(npcName, talkText, standing,uiTypes);
    }
}
