using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpButterfly : InteractionCreature
{
    public override void Interaction()
    {
        base.Interaction();
        player.UseItemEffect(UseItemType.HP_HEALTH, 30);
    }
}
