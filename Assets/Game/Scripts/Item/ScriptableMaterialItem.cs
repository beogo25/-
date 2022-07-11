using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialItem", menuName = "ScriptableObject/Item/MaterialItem")]

public class ScriptableMaterialItem : ScriptableObject
{
    public string itemName;
    public string contents;
    public int    value;
    public Sprite sprite;
}



