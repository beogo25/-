using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkBombus : InteractionCreature
{
    public override void Interaction()
    {
        base.Interaction();
        player.UseItemEffect(UseItemType.ATK_UP, 20);
    }
}
