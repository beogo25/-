using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField]
    private float     hp;
    private float     stamina;
    public  int       maxHp = 100;
    public  float     maxStamina = 100;

    private int       atk = 50;
    private int       def = 0;
    public static int gold = 10000;

    private int       burnCount;

    public  Slider    hpSlider;
    public  Slider    staminaSlider;

    private int       buffAtk;
    private int       buffDef;

    private IEnumerator atkIEnumerator    = null;
    private IEnumerator defIEnumerator    = null;

    [SerializeField]
    private EventReference   itemUse;
    [SerializeField] 
    private EventReference[] hitSound;

    private IEnumerator poisonIEnumerator = null;
    private IEnumerator burnIEnumerator   = null;
    private IEnumerator staminaHealthIEnumerator;

    private Player    player;
    private WaitForFixedUpdate stamanaHealthDelay = new WaitForFixedUpdate();
    private const float buffDuration = 180;

    private StatusAilment ailment = 0;
    public  Transform ailmentSprites;

    private Rigidbody rb;
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
    public float Hp
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
            staminaSlider.value = (float)stamina / maxStamina;
        }
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Hp = maxHp;
        Stamina = maxStamina;
        player = FindObjectOfType<Player>();
        player.rollDelegate += () =>
        {
            Stamina -= 15;
            burnCount--;
        };
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
            Debug.Log(player.questCount);
    }
    public void UseItemEffect(UseItemType useItemType, int value = 0)
    {
        switch (useItemType)
        {
            case UseItemType.HP_HEALTH:
                Hp += value;
                player.useParticleParent.GetChild(0).gameObject.SetActive(true);
                break;
            case UseItemType.ANTIDOTE:
                if (poisonIEnumerator != null)
                    StopCoroutine(poisonIEnumerator);
                Ailment &= ~StatusAilment.POISON;
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
        RuntimeManager.PlayOneShot(itemUse.Path);
    }
    public void PlayerHit(float damage, float knockBackPower, Vector3 position, AttackType attackType = AttackType.NORMAL)
    {
        Hp -= damage;
        if(damage > 0)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce((transform.position - position).normalized * knockBackPower, ForceMode.Impulse);
            player.HitDown();
            RuntimeManager.PlayOneShot(hitSound[0].Path);
        }

        switch (attackType)
        {
            case AttackType.POISON:
                if(poisonIEnumerator!=null)
                    StopCoroutine(poisonIEnumerator);
                poisonIEnumerator = Poison();
                StartCoroutine(poisonIEnumerator);   
                break;

            case AttackType.BURN:
                if (burnIEnumerator != null)
                    StopCoroutine(burnIEnumerator);
                burnIEnumerator = Burn();
                StartCoroutine(burnIEnumerator);
                break;
            default:
                break;
        }
    }
 
    public IEnumerator Poison()
    {   
        Ailment |= StatusAilment.POISON;
        for (int i = 0; i < 30; i++)
        {
            Hp -= 2;
            yield return new WaitForSecondsRealtime(1);
        }
        Ailment &= ~StatusAilment.POISON;
    }
    public IEnumerator Burn()
    {
        burnCount = 3;
        Ailment |= StatusAilment.BURN;
        for (int i = 0; i < 30; i++)
        {
            if (burnCount <= 0)
                break;
            Hp -= 1;
            yield return new WaitForSecondsRealtime(0.5f);
        }
        Ailment &= ~StatusAilment.BURN;
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
    
}
