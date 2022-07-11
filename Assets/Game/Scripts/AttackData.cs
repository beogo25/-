using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "ScriptableObject/AttackData")]
[System.Serializable]
public class AttackData : ScriptableObject
{
    public string    attackName;
    public EnumKey[] attackCommand;
    public float     attackDelay;
}
