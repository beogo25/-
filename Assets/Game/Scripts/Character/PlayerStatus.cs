using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField]
    private int hp;
    private int stamina;
    public  int maxHp=100;
    public  int maxStamina = 100;

    private int atk = 50;
    private int def = 0;
    public static int gold = 10000;

    public Slider hpSlider;
    public Slider staminaSlider;

    private int buffAtk;
    private int buffDef;

    private IEnumerator atkIEnumerator;
    private IEnumerator defIEnumerator;

    private WaitForSecondsRealtime tenSecond = new WaitForSecondsRealtime(10);

    public int Atk
    {
        get { return atk + buffAtk; }
        set { atk = value - buffAtk; }
    }
    public int Def
    {
        get { return def+ buffDef; }
        set { def = value - buffDef; }
    }
    public int Hp
    {
        get { return hp; }
        set 
        {
            if (value > maxHp)
                hp = maxHp;
            else
                hp = value;
            hpSlider.value = (float)hp/maxHp;    
        }
    }

    public int Stamina
    {
        get { return stamina; }
        set
        {
            if (value > maxStamina)
                stamina = maxStamina;
            else
                stamina = value;
            staminaSlider.value = (float)stamina /maxStamina;
        }
    }
    private void Awake()
    {
        Hp = maxHp;
        Stamina = maxStamina;   
    }
    public void Buff(UseItemType useItemType, int value)
    {
        switch (useItemType)
        {
            case UseItemType.ATK_UP:
                buffAtk = value;
                if(atkIEnumerator != null)
                    StopCoroutine(atkIEnumerator);
                atkIEnumerator = BuffIEnumerator(useItemType, value);
                StartCoroutine(atkIEnumerator);
                break;
            case UseItemType.DEF_UP:
                buffDef = value;
                if(defIEnumerator != null)
                    StopCoroutine(defIEnumerator);
                defIEnumerator = BuffIEnumerator(useItemType, value);
                StartCoroutine(defIEnumerator);
                break;
        }
    }
    
    public IEnumerator BuffIEnumerator(UseItemType useItemType, int value)
    {
        switch(useItemType)
        {
            case UseItemType.ATK_UP:
                buffAtk = value;
                yield return tenSecond;
                buffAtk = 0;
                break;
            case UseItemType.DEF_UP:
                buffDef = value;
                yield return tenSecond;
                buffDef = 0;
                break;
        }
        yield return null;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.X))
            Hp -= 10;
    }
}
