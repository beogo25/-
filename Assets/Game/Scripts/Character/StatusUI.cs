using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusUI : MonoBehaviour
{
    public PlayerStatus status;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI atk;
    public TextMeshProUGUI def;
    private void OnEnable()
    {
        ReFresh();
    }
    public void ReFresh()
    {
        hp.text = "HP  : " + status.Hp.ToString() + "/" + status.maxHp.ToString();
        atk.text = "ATK : " + status.Atk.ToString();
        def.text = "DEF : " + status.Def.ToString();
    }
}
