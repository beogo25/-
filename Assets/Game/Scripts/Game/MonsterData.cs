using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "ScriptableObject/MonsterData")]
[System.Serializable]
public class MonsterData : ScriptableObject
{
    public string monsterName;
    public Sprite monaterSprite;
    public int monsterNum;
}

