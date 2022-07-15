using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
    }

}
