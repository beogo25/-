using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
    }

}
