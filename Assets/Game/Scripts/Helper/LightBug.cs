using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBug : InteractionCreature
{
    public override void Interaction()
    {
        base.Interaction();
        player.UseItemEffect(UseItemType.ATK_UP, 20);
        player.UseItemEffect(UseItemType.DEF_UP, 20);
        player.UseItemEffect(UseItemType.HP_HEALTH, 30);
    }
}
