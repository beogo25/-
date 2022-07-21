using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "WeaponData", menuName = "ScriptableObject/WeaponData")]
[System.Serializable]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Material weaponMaterial;
    public Mesh weaponMesh;
}
