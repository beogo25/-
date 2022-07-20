using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField]
    private int hp;
    private float stamina;
    public int maxHp=100;
    public float maxStamina = 100;

    private int atk = 50;
    private int def = 0;
    public static int gold = 10000;

    public Slider hpSlider;
    public Slider staminaSlider;

    private int buffAtk;
    private int buffDef;

    private IEnumerator atkIEnumerator = null;
    private IEnumerator defIEnumerator = null;
    private IEnumerator staminaHealthIEnumerator;

    private Player player;
    private WaitForFixedUpdate stamanaHealthDelay = new WaitForFixedUpdate();
    private const float buffDuration = 180;

    private StatusAilment ailment=0;
    public  Transform ailmentSprites;
    public StatusAilment Ailment
    {
        get { return ailment; }
        set 
        {
            ailment = value;
            for(int i = 0; i < ailmentSprites.childCount; i++)
            {
                if (ailment.HasFlag((StatusAilment)(1 << i)))
                    ailmentSprites.GetChild(i).gameObject.SetActive(true);
                else
                    ailmentSprites.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

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

    public float Stamina
    {
        get { return stamina; }
        set
        {
            if(stamina > value)
            {
                if (staminaHealthIEnumerator != null)
                    StopCoroutine(staminaHealthIEnumerator);
                staminaHealthIEnumerator = StaminaHeath();
                StartCoroutine(staminaHealthIEnumerator);
            }
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
        player=FindObjectOfType<Player>();
    }
    public void UseItemEffect(UseItemType useItemType, int value=0)
    {
        switch (useItemType)
        {
            case UseItemType.HP_HEALTH:
                Hp += value;
                player.useParticleParent.GetChild(0).gameObject.SetActive(true);
                break;
            case UseItemType.ANTIDOTE:
                player.useParticleParent.GetChild(1).gameObject.SetActive(true);
                break;
            case UseItemType.ATK_UP:
                buffAtk = value;
                if(atkIEnumerator != null)
                    StopCoroutine(atkIEnumerator);
                atkIEnumerator = BuffIEnumerator(UseItemType.ATK_UP, value);
                StartCoroutine(atkIEnumerator);
                Ailment |= StatusAilment.ATTACK_UP;
                StartCoroutine(player.RimLight(new Color(1, 0.5f, 0.5f, 0)));
                player.useParticleParent.GetChild(2).gameObject.SetActive(true);
                break;
            case UseItemType.DEF_UP:
                buffDef = value;
                if(defIEnumerator != null)
                    StopCoroutine(defIEnumerator);
                defIEnumerator = BuffIEnumerator(UseItemType.DEF_UP, value);
                StartCoroutine(defIEnumerator);
                Ailment |= StatusAilment.DEFENCE_UP;
                StartCoroutine(player.RimLight(new Color(1, 0.8f, 0.4f, 0)));
                player.useParticleParent.GetChild(3).gameObject.SetActive(true);
                break;
        }
    }
    
    public IEnumerator StaminaHeath()
    {
        yield return new WaitForSecondsRealtime(2); 
        while(Stamina<maxStamina)
        {
            Stamina += 0.5f;
            yield return stamanaHealthDelay;
        }
    }

    public IEnumerator BuffIEnumerator(UseItemType useItemType, int value)
    {
        switch(useItemType)
        {
            case UseItemType.ATK_UP:
                buffAtk = value;
                yield return new WaitForSecondsRealtime(buffDuration);
                buffAtk = 0;
                Ailment &= ~StatusAilment.ATTACK_UP;
                break;
            case UseItemType.DEF_UP:
                buffDef = value;
                yield return new WaitForSecondsRealtime(buffDuration);
                buffDef = 0;
                Ailment &= ~StatusAilment.DEFENCE_UP;
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
