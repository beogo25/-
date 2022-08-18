using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefButterfly : InteractionCreature
{
    public override void Interaction()
    {
        base.Interaction();
        player.UseItemEffect(UseItemType.DEF_UP, 20);
    }
}
